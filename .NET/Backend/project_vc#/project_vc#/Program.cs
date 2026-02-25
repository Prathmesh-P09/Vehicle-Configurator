using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using project_vc_.Data;
using project_vc_.Services;
using System.Text;

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
                ValidateIssuer = false, // Set to true and set ValidIssuer if needed
                ValidateAudience = false 
            };
        });

        // App Services
        builder.Services.AddScoped<JwtUtil>(); // Inject config in ctor
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

        // app.UseHttpsRedirection(); // Disable for local dev compatibility with Java frontend

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
