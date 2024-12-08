using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Application.Services.Implementations;

public class MeasurementListService : IMeasurementListService
{
    public Task<List<Measurement>> GetFilteredMeasurementsByTenantAndSmartMeterAsync(Tenant tenant,
        SmartMeterId smartMeterId, DateTime? startAt = null,
        DateTime? endAt = null)
    {
        throw new NotImplementedException();
    }
}