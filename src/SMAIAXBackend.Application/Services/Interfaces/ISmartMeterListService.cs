using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface ISmartMeterListService
{
    Task<List<SmartMeterOverviewDto>> GetSmartMetersAsync();
    Task<SmartMeterOverviewDto> GetSmartMeterByIdAsync(Guid smartMeterId);
}