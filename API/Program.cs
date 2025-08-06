using API.Data;
using API.Services;
using API.Hubs;
using Microsoft.EntityFrameworkCore;
using API.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/api-.log", 
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "SQLite";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    switch (databaseProvider.ToUpper())
    {
        case "SQLSERVER":
            options.UseSqlServer(connectionString);
            break;
        case "SQLITE":
        default:
            options.UseSqlite(connectionString);
            break;
    }
});

builder.Services.AddScoped<TaskService>();

builder.Services.AddControllers();

// Add SignalR
builder.Services.AddSignalR();

// Add CORS for SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7154", "http://localhost:5154") // Frontend URLs
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<TaskValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("SignalRPolicy");

app.UseAuthorization();

app.MapControllers();
app.MapHub<TaskHub>("/taskhub");

// Ensure proper cleanup
try
{
    Log.Information("Starting application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
