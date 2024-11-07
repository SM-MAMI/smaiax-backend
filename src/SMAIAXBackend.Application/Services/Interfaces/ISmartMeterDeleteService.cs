namespace SMAIAXBackend.Application.Services.Interfaces;

public interface ISmartMeterDeleteService
{
    Task RemoveMetadataFromSmartMeterAsync(Guid smartMeterId, Guid metadataId);
}