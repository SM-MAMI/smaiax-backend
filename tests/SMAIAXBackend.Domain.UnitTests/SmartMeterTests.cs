using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
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

    [Test]
    public void GivenSmartMeterAndMetadata_WhenAddMetadata_ThenMetadataIsAdded()
    {
        // Given
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "SmartMeter", new UserId(Guid.NewGuid()));
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location("Some street", "Some city", "Some state", "Some country", Continent.Europe),
            4, smartMeter.Id);

        // When
        smartMeter.AddMetadata(metadata);

        // Then
        Assert.That(smartMeter.Metadata, Has.Count.EqualTo(1));
        Assert.That(smartMeter.Metadata[0], Is.EqualTo(metadata));
    }

    [Test]
    public void GivenSmartMeterAndMetadata_WhenAddMetadataTwice_ThenArgumentExceptionIsThrown()
    {
        // Given
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "SmartMeter", new UserId(Guid.NewGuid()));
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location("Some street", "Some city", "Some state", "Some country", Continent.Europe),
            4, smartMeter.Id);

        // When ... Then
        smartMeter.AddMetadata(metadata);
        Assert.Throws<ArgumentException>(() => smartMeter.AddMetadata(metadata));
    }
}