using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.Configurations;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class MeasurementRepository(
    ITenantDbContextFactory tenantDbContextFactory,
    IOptions<DatabaseConfiguration> databaseConfigOptions) : IMeasurementRepository
{
    public async Task<List<Measurement>> GetMeasurementsByTenantAndSmartMeterAsync(Tenant tenant,
        SmartMeterId smartMeterId,
        DateTime? startAt = null,
        DateTime? endAt = null)
    {
        TenantDbContext tenantSpecificDbContext = tenantDbContextFactory.CreateDbContext(tenant.DatabaseName,
            databaseConfigOptions.Value.SuperUsername, databaseConfigOptions.Value.SuperUserPassword);

        return await tenantSpecificDbContext.Measurements.AsNoTracking().Where(m => m.SmartMeterId.Equals(smartMeterId))
            .Where(m =>
                startAt == null
                || m.Timestamp >= startAt).Where(m => endAt == null || m.Timestamp < endAt).ToListAsync();
    }
}