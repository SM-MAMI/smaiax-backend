using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface ISmartMeterListService
{
    Task<List<SmartMeterOverviewDto>> GetSmartMetersAsync();
    Task<SmartMeterDto> GetSmartMeterByIdAsync(Guid smartMeterId);
}