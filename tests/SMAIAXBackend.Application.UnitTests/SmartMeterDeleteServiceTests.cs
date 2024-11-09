using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterDeleteServiceTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<ILogger<SmartMeterDeleteService>> _loggerMock;
    private SmartMeterDeleteService _smartMeterDeleteService;

    [SetUp]
    public void SetUp()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _loggerMock = new Mock<ILogger<SmartMeterDeleteService>>();
        _smartMeterDeleteService = new SmartMeterDeleteService(_smartMeterRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GivenSmartMeterIdAndMetadataIdAndUserId_WhenRemoveMetadataFromSmartMeter_ThenMetadataIsDeleted()
    {
        // Given
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter");
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location("Some Streetnaame", "Some City", "Some State", "Some Country", Continent.Africa), 4,
            smartMeter.Id);
        smartMeter.AddMetadata(metadata);

        _smartMeterRepositoryMock.Setup(x => x.GetSmartMeterByIdAsync(It.IsAny<SmartMeterId>()))
            .ReturnsAsync(smartMeter);

        // When
        await _smartMeterDeleteService.RemoveMetadataFromSmartMeterAsync(smartMeter.Id.Id, metadata.Id.Id);

        // Then
        Assert.That(smartMeter.Metadata, Is.Empty);
    }

    [Test]
    public void GivenSmartMeterIdAndNonExistentMetadataIdAndUserId_WhenRemoveMetadataFromSmartMeter_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var metadataId = new MetadataId(Guid.NewGuid());

        _smartMeterRepositoryMock.Setup(x => x.GetSmartMeterByIdAsync(smartMeterId))
            .ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(() =>
            _smartMeterDeleteService.RemoveMetadataFromSmartMeterAsync(smartMeterId.Id, metadataId.Id));
    }
}