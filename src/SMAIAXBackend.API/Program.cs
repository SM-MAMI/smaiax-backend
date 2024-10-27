using SMAIAXBackend.API;
using SMAIAXBackend.API.ApplicationConfigurations;
using SMAIAXBackend.Infrastructure.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConfigurations(builder.Configuration);
builder.Services.AddRepositoryConfigurations();
builder.Services.AddServiceConfigurations();
builder.Services.AddIdentityConfigurations();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddMqttServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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
    await applicationDbContext.Database.EnsureDeletedAsync();
    await applicationDbContext.Database.EnsureCreatedAsync();
    await applicationDbContext.SeedTestData();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

// For integration tests
public abstract partial class Program { }