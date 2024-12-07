using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.UnitTests.Model;

[TestFixture]
public class MeasurementTests
{
    [Test]
    public void GivenMeasurementDetails_WhenCreateMeasurement_ThenDetailsEquals()
    {
        // Given
        var timestamp = DateTime.UtcNow;
        var smartMeterId = new SmartMeterId(Guid.NewGuid());

        // When
        var measurement =
            Measurement.Create(smartMeterId, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, "test", timestamp);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(measurement.SmartMeterId, Is.EqualTo(smartMeterId));
            Assert.That(measurement.PositiveActivePower, Is.EqualTo(1));
            Assert.That(measurement.PositiveActiveEnergyTotal, Is.EqualTo(2));
            Assert.That(measurement.NegativeActivePower, Is.EqualTo(3));
            Assert.That(measurement.NegativeActiveEnergyTotal, Is.EqualTo(4));
            Assert.That(measurement.ReactiveEnergyQuadrant1Total, Is.EqualTo(5));
            Assert.That(measurement.ReactiveEnergyQuadrant3Total, Is.EqualTo(6));
            Assert.That(measurement.TotalPower, Is.EqualTo(7));
            Assert.That(measurement.CurrentPhase1, Is.EqualTo(8));
            Assert.That(measurement.VoltagePhase1, Is.EqualTo(9));
            Assert.That(measurement.CurrentPhase2, Is.EqualTo(10));
            Assert.That(measurement.VoltagePhase2, Is.EqualTo(11));
            Assert.That(measurement.CurrentPhase3, Is.EqualTo(12));
            Assert.That(measurement.VoltagePhase3, Is.EqualTo(13));
            Assert.That(measurement.Uptime, Is.EqualTo("test"));
            Assert.That(measurement.Timestamp, Is.EqualTo(timestamp));
        });
    }
}