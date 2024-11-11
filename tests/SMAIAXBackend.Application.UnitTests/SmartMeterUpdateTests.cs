using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterUpdateTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<ILogger<SmartMeterUpdateService>> _loggerMock;
    private SmartMeterUpdateService _smartMeterUpdateService;

    [SetUp]
    public void Setup()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _loggerMock = new Mock<ILogger<SmartMeterUpdateService>>();
        _smartMeterUpdateService = new SmartMeterUpdateService(_smartMeterRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GivenSmartMeterIdAndSmartMeterUpdateDtoAndUserId_WhenUpdateSmartMeter_ThenSmartMeterIdIsReturned()
    {
        // Given
        var smartMeterIdExpected = Guid.NewGuid();
        var smartMeterUpdateDto = new SmartMeterUpdateDto(smartMeterIdExpected, "Updated name");
        var smartMeter = SmartMeter.Create(new SmartMeterId(smartMeterIdExpected), "Name", []);

        _smartMeterRepositoryMock.Setup(repo =>
                repo.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterIdExpected)))
            .ReturnsAsync(smartMeter);

        // When
        Guid smartMeterIdActual =
            await _smartMeterUpdateService.UpdateSmartMeterAsync(smartMeterIdExpected, smartMeterUpdateDto);

        // Then
        Assert.That(smartMeterIdActual, Is.EqualTo(smartMeterIdExpected));
    }

    [Test]
    public void GivenNotMatchingSmartMeterIdsAndUserId_WhenUpdateSmartMeter_ThenSmartMeterIdMismatchExceptionIsThrown()
    {
        // Given
        var smartMeterIdExpected = Guid.NewGuid();
        var smartMeterUpdateDto = new SmartMeterUpdateDto(Guid.NewGuid(), "Updated name");

        // When ... Then
        Assert.ThrowsAsync<SmartMeterIdMismatchException>(async () =>
            await _smartMeterUpdateService.UpdateSmartMeterAsync(smartMeterIdExpected, smartMeterUpdateDto));
    }

    [Test]
    public void
        GivenNonExistentSmartMeterIdAndSmartMeterUpdateDtoAndUserId_WhenUpdateSmartMeter_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var smartMeterIdExpected = Guid.NewGuid();
        var smartMeterUpdateDto = new SmartMeterUpdateDto(smartMeterIdExpected, "Updated name");

        _smartMeterRepositoryMock.Setup(repo =>
                repo.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterIdExpected)))
            .ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(async () =>
            await _smartMeterUpdateService.UpdateSmartMeterAsync(smartMeterIdExpected, smartMeterUpdateDto));
    }
}