using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMAIAXBackend.API;
using SMAIAXBackend.Application.Interfaces;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Domain.Repositories.Transactions;
using SMAIAXBackend.Infrastructure;
using SMAIAXBackend.Infrastructure.Configurations;
using SMAIAXBackend.Infrastructure.DbContexts;
using SMAIAXBackend.Infrastructure.Messaging;
using SMAIAXBackend.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("smaiax-db"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Application Services.
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();

builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfiguration"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection("MQTT"));
builder.Services.AddSingleton<IMqttReader, MqttReader>(); 
builder.Services.AddHostedService<MessagingBackgroundService>();
var app = builder.Build();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("DockerDevelopment"))
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var userStoreDbContext = services.GetRequiredService<ApplicationDbContext>();
    await userStoreDbContext.Database.EnsureDeletedAsync();
    await userStoreDbContext.Database.EnsureCreatedAsync();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

// For integration tests
public abstract partial class Program
{
}