using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterCreateServiceTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<ITenantContextService> _tenantContextServiceMock;
    private Mock<ILogger<SmartMeterCreateService>> _loggerMock;
    private SmartMeterCreateService _smartMeterCreateService;

    [SetUp]
    public void Setup()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _tenantContextServiceMock = new Mock<ITenantContextService>();
        _loggerMock = new Mock<ILogger<SmartMeterCreateService>>();
        _smartMeterCreateService = new SmartMeterCreateService(_smartMeterRepositoryMock.Object,
            _tenantContextServiceMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GivenSmartMeterCreateDtoAndExistentUserId_WhenAddSmartMeter_ThenSmartMeterIdIsReturned()
    {
        // Given
        var smartMeterIdExpected = new SmartMeterId(Guid.NewGuid());
        var smartMeterCreateDto = new SmartMeterCreateDto("Test Smart Meter");
        var tenant = Tenant.Create(new TenantId(Guid.NewGuid()), "test", "test", "test");
        var user = User.Create(new UserId(Guid.NewGuid()), new Name("Test", "Test"), "test", "test@example.com",
            tenant.Id);

        _tenantContextServiceMock.Setup(service => service.GetCurrentTenantAsync());
        _smartMeterRepositoryMock.Setup(repo => repo.NextIdentity()).Returns(smartMeterIdExpected);

        // When
        var smartMeterIdActual =
            await _smartMeterCreateService.AddSmartMeterAsync(smartMeterCreateDto);

        // Then
        Assert.That(smartMeterIdActual, Is.EqualTo(smartMeterIdExpected.Id));
    }

    [Test]
    public async Task
        GivenSmartMeterIdAndMetadataCreateDtoAndExistentUserId_WhenAddMetadata_ThenSmartMeterIdIsReturned()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var metadataIdExpected = new MetadataId(Guid.NewGuid());
        var metadataCreateDto = new MetadataCreateDto(DateTime.Now,
            new LocationDto("Test Street", "Test City", "Test State", "Test Country", Continent.Europe), 1);
        var userId = new UserId(Guid.NewGuid());
        var smartMeter = SmartMeter.Create(smartMeterId, "Test Smart Meter", userId);

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userId.Id.ToString()))
            .ReturnsAsync(userId);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAndUserIdAsync(smartMeterId, userId))
            .ReturnsAsync(smartMeter);
        _smartMeterRepositoryMock.Setup(repo => repo.NextMetadataIdentity()).Returns(metadataIdExpected);

        // When
        var smartMeterIdActual =
            await _smartMeterCreateService.AddMetadataAsync(smartMeterId.Id, metadataCreateDto, userId.Id.ToString());

        // Then
        Assert.That(smartMeterIdActual, Is.EqualTo(smartMeterId.Id));
        _smartMeterRepositoryMock.Verify(repo => repo.UpdateAsync(smartMeter), Times.Once);
    }

    [Test]
    public void
        GivenSmartMeterIdAndMetadataCreateDtoAndNonexistentUserId_WhenAddMetadata_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var metadataCreateDto = new MetadataCreateDto(DateTime.Now,
            new LocationDto("Test Street", "Test City", "Test State", "Test Country", Continent.Europe), 1);
        var userId = new UserId(Guid.NewGuid());

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userId.Id.ToString()))
            .ReturnsAsync(userId);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAndUserIdAsync(smartMeterId, userId))
            .ReturnsAsync((SmartMeter)null!);

        // Then ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(async () =>
            await _smartMeterCreateService.AddMetadataAsync(smartMeterId.Id, metadataCreateDto, userId.Id.ToString()));
    }
}