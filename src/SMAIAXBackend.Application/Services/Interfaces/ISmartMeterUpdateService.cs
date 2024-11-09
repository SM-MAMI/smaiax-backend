using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface ISmartMeterUpdateService
{
    Task<Guid> UpdateSmartMeterAsync(Guid smartMeterIdExpected, SmartMeterUpdateDto smartMeterUpdateDto);
}