using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterDeleteServiceTests
{
    private Mock<IUserValidationService> _userValidationServiceMock;
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<ILogger<SmartMeterDeleteService>> _loggerMock;
    private SmartMeterDeleteService _smartMeterDeleteService;

    [SetUp]
    public void SetUp()
    {
        _userValidationServiceMock = new Mock<IUserValidationService>();
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _loggerMock = new Mock<ILogger<SmartMeterDeleteService>>();
        _smartMeterDeleteService = new SmartMeterDeleteService(
            _userValidationServiceMock.Object,
            _smartMeterRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task GivenSmartMeterIdAndMetadataIdAndUserId_WhenRemoveMetadataFromSmartMeter_ThenMetadataIsDeleted()
    {
        // Given
        var userId = new UserId(Guid.NewGuid());
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter", userId);
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location("Some Streetnaame", "Some City", "Some State", "Some Country", Continent.Africa), 4,
            smartMeter.Id);
        smartMeter.AddMetadata(metadata);

        _userValidationServiceMock.Setup(x => x.ValidateUserAsync(userId.Id.ToString())).ReturnsAsync(userId);
        _smartMeterRepositoryMock.Setup(x => x.GetSmartMeterByIdAndUserIdAsync(It.IsAny<SmartMeterId>(), userId))
            .ReturnsAsync(smartMeter);

        // When
        await _smartMeterDeleteService.RemoveMetadataFromSmartMeterAsync(smartMeter.Id.Id, metadata.Id.Id, userId.Id.ToString());

        // Then
        Assert.That(smartMeter.Metadata, Is.Empty);
        _smartMeterRepositoryMock.Verify(x => x.UpdateAsync(smartMeter), Times.Once);
    }

    [Test]
    public void GivenSmartMeterIdAndNonExistentMetadataIdAndUserId_WhenRemoveMetadataFromSmartMeter_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var userId = new UserId(Guid.NewGuid());
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var metadataId = new MetadataId(Guid.NewGuid());

        _userValidationServiceMock.Setup(x => x.ValidateUserAsync(userId.Id.ToString())).ReturnsAsync(userId);
        _smartMeterRepositoryMock.Setup(x => x.GetSmartMeterByIdAndUserIdAsync(smartMeterId, userId))
            .ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(() =>
            _smartMeterDeleteService.RemoveMetadataFromSmartMeterAsync(smartMeterId.Id, metadataId.Id, userId.Id.ToString()));
    }
}