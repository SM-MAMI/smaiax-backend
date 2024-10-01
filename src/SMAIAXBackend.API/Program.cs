using Microsoft.EntityFrameworkCore;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Infrastructure.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserStoreDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("user-store"));
});

// Application Services.
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var userStoreDbContext = services.GetRequiredService<UserStoreDbContext>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    await userStoreDbContext.Database.EnsureDeletedAsync();
    await userStoreDbContext.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();