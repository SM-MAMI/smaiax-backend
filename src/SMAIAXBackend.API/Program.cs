using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.API.ApplicationConfigurations;
using SMAIAXBackend.API.Endpoints.Authentication;
using SMAIAXBackend.API.Endpoints.Policy;
using SMAIAXBackend.API.Endpoints.PolicyRequest;
using SMAIAXBackend.API.Endpoints.SmartMeter;
using SMAIAXBackend.API.Middlewares;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.Configurations;
using SMAIAXBackend.Infrastructure.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConfigurations(builder.Configuration);
builder.Services.AddRepositoryConfigurations();
builder.Services.AddServiceConfigurations();
builder.Services.AddIdentityConfigurations();
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandlerMiddleware>();
builder.Services.AddHttpContextAccessor();

// Add Swagger if in development environment
if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("DockerDevelopment"))
{
    builder.Configuration.AddJsonFile("Properties/launchSettings.json", optional: true, reloadOnChange: true);
    builder.Services.AddSwaggerConfigurations(builder.Configuration);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("DockerDevelopment"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");

    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();
    var tenantRepository = services.GetRequiredService<ITenantRepository>();
    var tenantDbContextFactory = services.GetRequiredService<ITenantDbContextFactory>();

    await applicationDbContext.Database.EnsureDeletedAsync();
    await applicationDbContext.Database.EnsureCreatedAsync();
    await applicationDbContext.SeedTestData();

    // Create a database for the test user with test data for development
    var dbConfig = app.Configuration.GetSection("DatabaseConfiguration").Get<DatabaseConfiguration>();
    var testUsername = app.Configuration.GetValue<string>("TestUser:Username");
    var testUserPassword = app.Configuration.GetValue<string>("TestUser:Password");
    var testUserDatabase = app.Configuration.GetValue<string>("TestUser:Database");

    var tenantDbContext =
        tenantDbContextFactory.CreateDbContext("tenant_1_db", dbConfig!.SuperUsername, dbConfig.SuperUserPassword);
    await tenantDbContext.Database.EnsureDeletedAsync();
    await using (var deleteUserCommand = applicationDbContext.Database.GetDbConnection().CreateCommand())
    {
        deleteUserCommand.CommandText = "DROP ROLE IF EXISTS johndoe;";
        await applicationDbContext.Database.OpenConnectionAsync();
        await deleteUserCommand.ExecuteNonQueryAsync();
        await applicationDbContext.Database.CloseConnectionAsync();
    }
    await tenantRepository.CreateDatabaseForTenantAsync(testUserDatabase!, testUsername!, testUserPassword!);
    await tenantDbContext.SeedTestData();
}

app.UseAuthorization();
app.UseMiddleware<JwtClaimMiddleware>();
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapAuthenticationEndpoints()
    .MapSmartMeterEndpoints()
    .MapPolicyEndpoints()
    .MapPolicyRequestEndpoints();

await app.RunAsync();

// For integration tests
[ExcludeFromCodeCoverage]
public abstract partial class Program
{
}