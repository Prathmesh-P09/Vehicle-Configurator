using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using project_vc_.Data;
using project_vc_.Services;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace project_vc_;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        // DB Context
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // Auth / JWT
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // Disable mapping standard JWT claims to XML namespaces
        var jwtKey = builder.Configuration["Jwt:Secret"] ?? "default_secret_key_long_enough";
        var key = Encoding.ASCII.GetBytes(jwtKey);

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                NameClaimType = "name", // Match the literal 'name' claim seen in the logs
                RoleClaimType = "role",
                ClockSkew = TimeSpan.FromMinutes(5) 
            };
            x.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                    Console.WriteLine($"[JWT Auth Failed] Type: {context.Exception.GetType().Name}, Message: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("[JWT Token Validated] Claims found:");
                    foreach (var claim in context.Principal?.Claims ?? Enumerable.Empty<Claim>())
                    {
                        Console.WriteLine($"  -> {claim.Type}: {claim.Value}");
                    }
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    Console.WriteLine($"[JWT Message Received] Header: {(string.IsNullOrEmpty(authHeader) ? "MISSING" : "PRESENT")}");
                    return Task.CompletedTask;
                }
            };
        })
        .AddCookie("ExternalCookie")
        .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "MISSING_CLIENT_ID";
            googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "MISSING_CLIENT_SECRET";
            googleOptions.SignInScheme = "ExternalCookie";
            googleOptions.CallbackPath = "/login/oauth2/code/google"; // Match Java default
        })
        .AddFacebook(facebookOptions =>
        {
            facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"] ?? "MISSING_APP_ID";
            facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? "MISSING_APP_SECRET";
            facebookOptions.SignInScheme = "ExternalCookie";
            facebookOptions.CallbackPath = "/login/oauth2/code/facebook"; // Match Java default
        });

        // App Services
        builder.Services.AddScoped<IJwtUtil, JwtUtil>(); // Inject config in ctor
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IVehicleService, VehicleService>();
        builder.Services.AddScoped<IInvoiceService, InvoiceService>();

        builder.Services.AddControllers();
        
        // Swagger with Auth
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vehicle Configurator API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });

        // CORS - Allow Credentials for Frontend
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                b => b.WithOrigins("http://localhost:5173") // Vite default port
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials());
        });

        var app = builder.Build();

        // Ensure DB Created
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowAll");

        // DEBUG MIDDLEWARE: Log all headers
        app.Use(async (context, next) =>
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            var authHeader = context.Request.Headers["Authorization"].ToString();
            logger.LogInformation($"[Request] {context.Request.Method} {context.Request.Path} - Auth Header: {(string.IsNullOrEmpty(authHeader) ? "MISSING" : "PRESENT")}");
            await next();
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
