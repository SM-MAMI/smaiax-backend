using System.Diagnostics.CodeAnalysis;

using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.ApplicationConfigurations;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static void AddServiceConfigurations(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ISmartMeterCreateService, SmartMeterCreateService>();
        services.AddScoped<ISmartMeterListService, SmartMeterListService>();
        services.AddScoped<ISmartMeterUpdateService, SmartMeterUpdateService>();
        services.AddScoped<ISmartMeterDeleteService, SmartMeterDeleteService>();
        services.AddScoped<ITenantContextService, TenantContextService>();
        services.AddScoped<IPolicyCreateService, PolicyCreateService>();
        services.AddScoped<IPolicyRequestCreateService, PolicyRequestCreateService>();
        services.AddScoped<IUserService, UserService>();
    }
}