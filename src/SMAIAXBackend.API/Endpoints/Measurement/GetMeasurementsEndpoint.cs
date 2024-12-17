using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.Measurement;

public static class GetMeasurementsEndpoint
{
    public static async Task<Ok<List<MeasurementRawDto>>> Handle(IMeasurementListService measurementListService,
        [FromQuery] Guid smartMeterId,
        [FromQuery] DateTime startAt, [FromQuery] DateTime endAt)
    {
        var measurements = await measurementListService.GetMeasurementsBySmartMeterAsync(smartMeterId, startAt, endAt);

        return TypedResults.Ok(measurements);
    }
}