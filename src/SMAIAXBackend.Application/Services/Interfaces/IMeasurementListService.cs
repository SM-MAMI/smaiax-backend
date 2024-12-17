using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IMeasurementListService
{
    /// <summary>
    ///     Get the time-filtered measurements of a tenant by smart meter.
    /// </summary>
    /// <param name="smartMeterId">The specific smart meter id.</param>
    /// <param name="startAt">Timestamp filter. Data with a timestamp newer/greater than or equal to startAt will be returned.</param>
    /// <param name="endAt">Timestamp filter. Data with a timestamp older/smaller than or equal to endAt will be returned.</param>
    /// <returns>All measurements between given limitations</returns>
    Task<List<MeasurementRawDto>> GetMeasurementsBySmartMeterAsync(Guid smartMeterId,
        DateTime startAt, DateTime endAt);
}