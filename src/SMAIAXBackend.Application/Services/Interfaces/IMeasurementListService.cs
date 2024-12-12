using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IMeasurementListService
{
    /// <summary>
    ///     Get all or filtered measurements of a tenant by smart meter.
    /// </summary>
    /// <param name="smartMeterId">The specific smart meter id.</param>
    /// <param name="startAt">
    ///     Optional timestamp filter. Data with a timestamp newer/greater than or equal to startAt will be
    ///     returned.
    /// </param>
    /// <param name="endAt">Optional timestamp filter. Data with a timestamp older/smaller than endAt will be returned.</param>
    /// <returns>All measurements between given limitations or by default measurements of one day.</returns>
    Task<List<MeasurementRawDto>> GetFilteredMeasurementsByTenantAndSmartMeterAsync(Guid smartMeterId,
        DateTime? startAt = null, DateTime? endAt = null);
}