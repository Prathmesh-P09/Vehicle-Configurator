using Microsoft.EntityFrameworkCore;
using VehicleConfigurator.ConsoleApp.Data;
using VehicleConfigurator.ConsoleApp.Data.Repositories;
using VehicleConfigurator.ConsoleApp.Services;
using VehicleConfigurator.ConsoleApp.Utils;

var builder = WebApplication.CreateBuilder(args);

// FORCE PORT 8080
builder.WebHost.UseUrls("http://*:8080");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
builder.Services.AddScoped<ISgMfgMasterRepository, SgMfgMasterRepository>();
builder.Services.AddScoped<IModelRepository, ModelRepository>();
builder.Services.AddScoped<IVehicleDetailRepository, VehicleDetailRepository>();
builder.Services.AddScoped<IAlternateComponentRepository, AlternateComponentRepository>();
builder.Services.AddScoped<IDefaultConfigRepository, DefaultConfigRepository>();
builder.Services.AddScoped<IInvoiceHeaderRepository, InvoiceHeaderRepository>();
builder.Services.AddScoped<IInvoiceDetailRepository, InvoiceDetailRepository>();
builder.Services.AddScoped<IComponentRepository, ComponentRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWelcomeService, WelcomeService>();
builder.Services.AddScoped<IVehicleManager, VehicleManager>();
builder.Services.AddScoped<IDefaultConfigService, DefaultConfigService>();
builder.Services.AddScoped<IInvoiceManager, InvoiceManager>();

// Utils
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IPdfService, PdfService>();

// Caching
builder.Services.AddMemoryCache();

// CORS
// CORS - DEBUG MODE: ALLOW ANY
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin() // For debugging "Server Unreachable"
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// ENABLE SWAGGER IN ALL ENVIRONMENTS (Production included)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

// Redirect Root to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
