using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IMeasurementListService
{
    /// <summary>
    ///     Get all or filtered measurements by tenant.
    /// </summary>
    /// <param name="tenant">The specific tenant.</param>
    /// <param name="smartMeterId">The specific smart meter id.</param>
    /// <param name="startAt">
    ///     Optional timestamp filter. Data with a timestamp newer/greater than or equal to startAt will be
    ///     returned.
    /// </param>
    /// <param name="endAt">Optional timestamp filter. Data with a timestamp older/smaller than endAt will be returned.</param>
    /// <returns>All or filtered measurements.</returns>
    Task<List<Measurement>> GetFilteredMeasurementsByTenantAndSmartMeterAsync(Tenant tenant, SmartMeterId smartMeterId,
        DateTime? startAt = null, DateTime? endAt = null);
}