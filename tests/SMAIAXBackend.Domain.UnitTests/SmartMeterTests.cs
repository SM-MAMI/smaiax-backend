using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.UnitTests;

[TestFixture]
public class SmartMeterTests
{
    [Test]
    public void GivenSmartMeterDetails_WhenCreateSmartMeter_ThenDetailsEquals()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var name = "SmartMeter";
        var userId = new UserId(Guid.NewGuid());

        // When
        var smartMeter = SmartMeter.Create(smartMeterId, name, userId);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(smartMeter.Id, Is.EqualTo(smartMeterId));
            Assert.That(smartMeter.Name, Is.EqualTo(name));
            Assert.That(smartMeter.Metadata, Is.Empty);
            Assert.That(smartMeter.UserId, Is.EqualTo(userId));
            Assert.That(smartMeter.Policies, Is.Empty);
        });
    }
}