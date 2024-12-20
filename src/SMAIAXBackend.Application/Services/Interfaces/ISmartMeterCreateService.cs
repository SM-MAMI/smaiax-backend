using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface ISmartMeterCreateService
{
    Task<Guid> AddSmartMeterAsync(SmartMeterCreateDto smartMeterCreateDto);
    Task<Guid> AddMetadataAsync(Guid smartMeterId, MetadataCreateDto metadataCreateDto);
}