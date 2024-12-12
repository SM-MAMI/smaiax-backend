using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.API.Endpoints.Measurement;

public static class MeasurementEndpoints
{
    public static WebApplication MapMeasurementEndpoints(this WebApplication app)
    {
        const string contentType = "application/json";
        RouteGroupBuilder group = app.MapGroup("/api/measurements").WithTags("Measurement").RequireAuthorization();
        group.MapGet("/", GetMeasurementsEndpoint.Handle)
            .WithName("getMeasurements")
            .Produces<List<MeasurementRawDto>>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}