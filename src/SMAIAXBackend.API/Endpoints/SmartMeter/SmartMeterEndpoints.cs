using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class SmartMeterEndpoints
{
    public static WebApplication MapSmartMeterEndpoints(this WebApplication app)
    {
        const string contentType = "application/json";
        var group = app.MapGroup("/api/smartMeters")
            .WithTags("SmartMeter")
            .RequireAuthorization();

        group.MapGet("/", GetSmartMetersEndpoint.Handle)
            .WithName("getSmartMeters")
            .Produces<List<SmartMeterOverviewDto>>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", GetSmartMeterByIdEndpoint.Handle)
            .WithName("getSmartMeterById")
            .Produces<SmartMeterOverviewDto>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", AddSmartMeterEndpoint.Handle)
            .WithName("addSmartMeter")
            .Accepts<SmartMeterCreateDto>(contentType)
            .Produces<Guid>(StatusCodes.Status201Created, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:guid}", UpdateSmartMeterEndpoint.Handle)
            .WithName("updateSmartMeter")
            .Accepts<SmartMeterUpdateDto>(contentType)
            .Produces<Guid>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}