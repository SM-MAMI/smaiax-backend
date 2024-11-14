using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.UnitTests;

[TestFixture]
public class MeasurementTests
{
    [Test]
    public void GivenMeasurementDetails_WhenCreateMeasurement_ThenDetailsEquals()
    {
        // Given
        var measurementId = new MeasurementId(Guid.NewGuid());
        var timestamp = DateTime.UtcNow;
        var measurementData = new MeasurementData(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", timestamp);
        var smartMeterId = new SmartMeterId(Guid.NewGuid());

        // When
        var measurement = Measurement.Create(measurementId, timestamp, measurementData, smartMeterId);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(measurement.Id, Is.EqualTo(measurementId));
            Assert.That(measurement.Timestamp, Is.EqualTo(timestamp));
            Assert.That(measurement.Data, Is.EqualTo(measurementData));
            Assert.That(measurement.SmartMeterId, Is.EqualTo(smartMeterId));
        });
    }
}