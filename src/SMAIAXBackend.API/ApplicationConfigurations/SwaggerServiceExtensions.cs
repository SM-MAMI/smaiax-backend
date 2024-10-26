using Microsoft.OpenApi.Models;

namespace SMAIAXBackend.API.ApplicationConfigurations;

public static class SwaggerServiceExtensions
{
    public static void AddSwaggerConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "SMAIAX Backend API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        In = ParameterLocation.Header,
                        Name = "Bearer"
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