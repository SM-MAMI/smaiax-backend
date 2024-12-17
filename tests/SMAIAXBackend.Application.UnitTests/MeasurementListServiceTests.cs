using Moq;

using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class MeasurementListServiceTests
{
    private Mock<IMeasurementRepository> _measurementRepositoryMock;
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private MeasurementListService _measurementListService;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _measurementRepositoryMock = new Mock<IMeasurementRepository>();
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _measurementListService =
            new MeasurementListService(_measurementRepositoryMock.Object, _smartMeterRepositoryMock.Object);
    }

    [Test]
    public async Task
        GivenSmartMeterIdAndStartAtAndEndAt_WhenGetMeasurementsBySmartMeterAsync_ThenReturnExpectedMeasurements()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var startAt = DateTime.UtcNow;
        var endAt = DateTime.UtcNow.AddHours(1);
        var smartMeter = SmartMeter.Create(smartMeterId, "Smart Meter 1", []);
        var measurementsExpected = new List<Measurement>
        {
            Measurement.Create(
                new SmartMeterId(Guid.NewGuid()),
                100.0,
                200.0,
                50.0,
                75.0,
                30.0,
                45.0,
                500.0,
                10.0,
                220.0,
                15.0,
                230.0,
                20.0,
                240.0,
                "1000h",
                DateTime.UtcNow)
        };

        _smartMeterRepositoryMock.Setup(x => x.GetSmartMeterByIdAsync(smartMeter.Id)).ReturnsAsync(smartMeter);
        _measurementRepositoryMock
            .Setup(x => x.GetMeasurementsBySmartMeterAsync(smartMeter.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(measurementsExpected);

        // When
        var measurementsActual = await _measurementListService.GetMeasurementsBySmartMeterAsync(smartMeterId.Id, startAt, endAt);

        // Then
        Assert.That(measurementsActual, Is.Not.Null);
        Assert.That(measurementsActual, Has.Count.EqualTo(measurementsExpected.Count));
    }

    [Test]
    public void
        GivenSmartMeterIdAndStartAtAndEndAt_WhenGetMeasurementsBySmartMeterAsync_ThenThrowSmartMeterNotFoundException()
    {
        // Given
        var smartMeterId = Guid.NewGuid();
        var startAt = DateTime.UtcNow;
        var endAt = DateTime.UtcNow.AddHours(1);

        _smartMeterRepositoryMock.Setup(x => x.GetSmartMeterByIdAsync(It.IsAny<SmartMeterId>())).ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(async () =>
            await _measurementListService.GetMeasurementsBySmartMeterAsync(smartMeterId, startAt, endAt)
        );
    }

    [Test]
    public void
        GivenSmartMeterIdAndStartAtAndEndAt_WhenGetMeasurementsBySmartMeterAsync_ThenThrowInvalidTimeRangeException()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var startAt = DateTime.UtcNow;
        var endAt = DateTime.UtcNow.AddHours(-1);
        var smartMeter = SmartMeter.Create(smartMeterId, "Smart Meter 1", []);

        _smartMeterRepositoryMock.Setup(x => x.GetSmartMeterByIdAsync(smartMeter.Id)).ReturnsAsync(smartMeter);

        // When ... Then
        Assert.ThrowsAsync<InvalidTimeRangeException>(async () =>
            await _measurementListService.GetMeasurementsBySmartMeterAsync(smartMeterId.Id, startAt, endAt)
        );
    }
}