using System.Diagnostics.CodeAnalysis;

using Microsoft.OpenApi.Models;

namespace SMAIAXBackend.API.ApplicationConfigurations;

[ExcludeFromCodeCoverage]
public static class SwaggerServiceExtensions
{
    public static void AddSwaggerConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        const string securitySchemeName = "Bearer";
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "SMAIAX Backend API", Version = "v1" });

            options.AddSecurityDefinition(securitySchemeName, new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = securitySchemeName
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = securitySchemeName },
                        In = ParameterLocation.Header,
                        Name = securitySchemeName
                    },
                    new List<string>()
                }
            });

            var httpProfileUrl = configuration["profiles:http:applicationUrl"];
            if (!string.IsNullOrEmpty(httpProfileUrl))
            {
                options.AddServer(new OpenApiServer { Url = httpProfileUrl.Trim(), Description = "Development server" });
            }
        });
    }
}