using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class MeasurementRepository(
    TenantDbContext tenantDbContext,
    ILogger<MeasurementRepository> logger) : IMeasurementRepository
{
    public async Task<List<Measurement>> GetMeasurementsByTenantAndSmartMeterAsync(SmartMeterId smartMeterId,
        DateTime? startAt = null,
        DateTime? endAt = null)
    {
        if (endAt > startAt)
        {
            logger.LogError("Argument 'endAt' must be before 'startAt'.");
            throw new ArgumentException("Argument 'endAt' must be before 'startAt'.");
        }

        // If endAt is null, by default one month is returned, either from startAt or from DateTime.UtcNow.
        endAt ??= startAt?.AddDays(-1) ?? DateTime.UtcNow.AddDays(-1);

        return await tenantDbContext.Measurements.AsNoTracking().Where(m => m.SmartMeterId.Equals(smartMeterId))
            .Where(m =>
                startAt == null
                || m.Timestamp >= startAt).Where(m => endAt == null || m.Timestamp < endAt)
            .OrderByDescending(m => m.Timestamp).ToListAsync();
    }
}