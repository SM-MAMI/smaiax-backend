using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Repositories;

public interface ISmartMeterRepository
{
    SmartMeterId NextIdentity();
    Task AddAsync(SmartMeter meter, Tenant tenant);
    Task<List<SmartMeter>> GetSmartMetersAsync(Tenant tenant);
    Task<SmartMeter?> GetSmartMeterByIdAsync(SmartMeterId smartMeterId, Tenant tenant);
    Task UpdateAsync(SmartMeter smartMeter, Tenant tenant);
}