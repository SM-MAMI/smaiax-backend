using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.IntegrationTests.RepositoryTests;

[TestFixture]
public class MeasurementRepositoryTests : TestBase
{
    [Test]
    public async Task GivenMeasurementsInRepository_WhenGetMeasurements_ThenMeasurementsAreReturned()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd"));
        var startAt = DateTime.UtcNow.AddDays(-2);
        var endAt = DateTime.UtcNow;

        // When
        var measurementsActual =
            await _measurementRepository.GetMeasurementsBySmartMeterAsync(smartMeterId, startAt, endAt);

        // Then
        Assert.That(measurementsActual, Is.Not.Null);
        Assert.That(measurementsActual, Has.Count.EqualTo(1));
        Assert.That(measurementsActual[0].SmartMeterId, Is.EqualTo(smartMeterId));
    }

    [Test]
    public async Task GivenUnknownSmartMeterId_WhenGetMeasurements_ThenMeasurementsAreEmpty()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.Parse("6f064ac0-f14f-448e-969e-0f5b2884b631"));
        var startAt = DateTime.UtcNow.AddDays(-1);
        var endAt = DateTime.UtcNow;

        // When
        var measurementsActual =
            await _measurementRepository.GetMeasurementsBySmartMeterAsync(smartMeterId, startAt, endAt);

        // Then
        Assert.That(measurementsActual, Is.Not.Null);
        Assert.That(measurementsActual, Is.Empty);
    }

    [Test]
    public async Task GivenSmartMeterInRepository_WhenGetMeasurements_ThenMeasurementsAreNotInRange()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd"));
        var endAt = DateTime.UtcNow.AddDays(-1);
        var startAt = DateTime.UtcNow;

        // When
        var measurementsActual =
            await _measurementRepository.GetMeasurementsBySmartMeterAsync(smartMeterId, startAt, endAt);

        // Then
        Assert.That(measurementsActual, Is.Not.Null);
        Assert.That(measurementsActual, Is.Empty);
    }
}