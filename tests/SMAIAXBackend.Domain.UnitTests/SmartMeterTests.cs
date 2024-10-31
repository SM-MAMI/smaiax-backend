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
        const string name = "SmartMeter";
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

    [Test]
    public void GivenSmartMeterAndNewName_WhenUpdateSmartMeter_ThenNameIsUpdated()
    {
        // Given
        const string name = "SmartMeter";
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), name, new UserId(Guid.NewGuid()));
        const string nameExpected = "Updated name";

        // When
        smartMeter.Update(nameExpected);

        // Then
        Assert.That(smartMeter.Name, Is.EqualTo(nameExpected));
        Assert.That(smartMeter.Name, Is.Not.EqualTo(name));
    }
}