using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Repositories;

public interface IMeasurementRepository
{
    /// <summary>
    ///     Reads all measurements of a tenant or filtered measurements when the parameter is given.
    /// </summary>
    /// <param name="smartMeterId">The specific smart meter id.</param>
    /// <param name="startAt">
    ///     Optional timestamp filter. Data with a timestamp newer/greater than or equal to startAt will be
    ///     returned.
    /// </param>
    /// <param name="endAt">Optional timestamp filter. Data with a timestamp older/smaller than endAt will be returned.</param>
    /// <returns>All measurements between given limitations or by default measurements of one day.</returns>
    Task<List<Measurement>> GetMeasurementsByTenantAndSmartMeterAsync(SmartMeterId smartMeterId,
        DateTime? startAt = null, DateTime? endAt = null);
}