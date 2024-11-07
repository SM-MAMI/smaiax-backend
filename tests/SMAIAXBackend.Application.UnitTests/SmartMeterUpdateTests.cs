using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterUpdateTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<IUserValidationService> _userValidationServiceMock;
    private Mock<ILogger<SmartMeterUpdateService>> _loggerMock;
    private SmartMeterUpdateService _smartMeterUpdateService;

    [SetUp]
    public void Setup()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _userValidationServiceMock = new Mock<IUserValidationService>();
        _loggerMock = new Mock<ILogger<SmartMeterUpdateService>>();
        _smartMeterUpdateService = new SmartMeterUpdateService(_smartMeterRepositoryMock.Object,
            _userValidationServiceMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GivenSmartMeterIdAndSmartMeterUpdateDtoAndUserId_WhenUpdateSmartMeter_ThenSmartMeterIdIsReturned()
    {
        // Given
        var smartMeterIdExpected = Guid.NewGuid();
        var smartMeterUpdateDto = new SmartMeterUpdateDto(smartMeterIdExpected, "Updated name");
        var userId = Guid.NewGuid();
        var smartMeter = SmartMeter.Create(new SmartMeterId(smartMeterIdExpected), "Name", new UserId(userId));

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userId.ToString()))
            .ReturnsAsync(new UserId(userId));
        _smartMeterRepositoryMock.Setup(repo =>
                repo.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterIdExpected), new UserId(userId)))
            .ReturnsAsync(smartMeter);

        // When
        Guid smartMeterIdActual =
            await _smartMeterUpdateService.UpdateSmartMeterAsync(smartMeterIdExpected, smartMeterUpdateDto,
                userId.ToString());

        // Then
        Assert.That(smartMeterIdActual, Is.EqualTo(smartMeterIdExpected));
        _smartMeterRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<SmartMeter>()), Times.Once);
    }

    [Test]
    public void GivenNotMatchingSmartMeterIdsAndUserId_WhenUpdateSmartMeter_ThenSmartMeterIdMismatchExceptionIsThrown()
    {
        // Given
        var smartMeterIdExpected = Guid.NewGuid();
        var smartMeterUpdateDto = new SmartMeterUpdateDto(Guid.NewGuid(), "Updated name");
        var userId = Guid.NewGuid();

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userId.ToString()))
            .ReturnsAsync(new UserId(userId));

        // When ... Then
        Assert.ThrowsAsync<SmartMeterIdMismatchException>(async () =>
            await _smartMeterUpdateService.UpdateSmartMeterAsync(smartMeterIdExpected, smartMeterUpdateDto, userId.ToString()));
    }

    [Test]
    public void GivenNonExistentSmartMeterIdAndSmartMeterUpdateDtoAndUserId_WhenUpdateSmartMeter_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var smartMeterIdExpected = Guid.NewGuid();
        var smartMeterUpdateDto = new SmartMeterUpdateDto(smartMeterIdExpected, "Updated name");
        var userId = Guid.NewGuid();

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userId.ToString()))
            .ReturnsAsync(new UserId(userId));
        _smartMeterRepositoryMock.Setup(repo =>
                repo.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterIdExpected), new UserId(userId)))
            .ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(async () =>
            await _smartMeterUpdateService.UpdateSmartMeterAsync(smartMeterIdExpected, smartMeterUpdateDto, userId.ToString()));
    }
}