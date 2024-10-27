using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface ISmartMeterListService
{
    Task<List<SmartMeterOverviewDto>> GetSmartMetersByUserIdAsync(string? userId);
    Task<SmartMeterOverviewDto> GetSmartMeterByIdAndUserIdAsync(Guid smartMeterId, string? userId);
}