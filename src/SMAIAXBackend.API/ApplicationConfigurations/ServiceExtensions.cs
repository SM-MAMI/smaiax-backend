using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.ApplicationConfigurations;

public static class ServiceExtensions
{
    public static void AddServiceConfigurations(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ISmartMeterCreateService, SmartMeterCreateService>();
    }
}