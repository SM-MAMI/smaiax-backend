using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Repositories;

public interface ISmartMeterRepository
{
    SmartMeterId NextIdentity();
    Task AddAsync(SmartMeter meter);
    Task<List<SmartMeter>> GetSmartMetersByUserIdAsync(UserId userId);
    Task<SmartMeter?> GetSmartMeterByIdAndUserIdAsync(SmartMeterId smartMeterId, UserId userId);
}