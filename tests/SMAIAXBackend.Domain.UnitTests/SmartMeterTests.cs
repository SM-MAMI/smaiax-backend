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
        const string name = "SmartMeter";

        // When
        var smartMeter = SmartMeter.Create(smartMeterId, name);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(smartMeter.Id, Is.EqualTo(smartMeterId));
            Assert.That(smartMeter.Name, Is.EqualTo(name));
            Assert.That(smartMeter.Metadata, Is.Empty);
        });
    }

    [Test]
    public void GivenSmartMeterAndNewName_WhenUpdateSmartMeter_ThenNameIsUpdated()
    {
        // Given
        const string name = "SmartMeter";
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), name);
        const string nameExpected = "Updated name";

        // When
        smartMeter.Update(nameExpected);

        // Then
        Assert.That(smartMeter.Name, Is.EqualTo(nameExpected));
        Assert.That(smartMeter.Name, Is.Not.EqualTo(name));
    }

    [Test]
    public void GivenSmartMeterAndMetadata_WhenAddMetadata_ThenMetadataIsAdded()
    {
        // Given
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "SmartMeter");
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
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "SmartMeter");
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location("Some street", "Some city", "Some state", "Some country", Continent.Europe),
            4, smartMeter.Id);

        // When ... Then
        smartMeter.AddMetadata(metadata);
        Assert.Throws<ArgumentException>(() => smartMeter.AddMetadata(metadata));
    }

    [Test]
    public void GivenSmartMeterAndMetadataId_WhenRemoveMetadata_ThenMetadataIsRemoved()
    {
        // Given
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "SmartMeter");
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location("Some street", "Some city", "Some state", "Some country", Continent.Europe),
            4, smartMeter.Id);
        smartMeter.AddMetadata(metadata);

        // When
        smartMeter.RemoveMetadata(metadata.Id);

        // Then
        Assert.That(smartMeter.Metadata, Is.Empty);
    }

    [Test]
    public void GivenSmartMeterAndNonExistentMetadataId_WhenRemoveMetadata_ThenArgumentExceptionIsThrown()
    {
        // Given
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "SmartMeter");

        // When ... Then
        Assert.Throws<ArgumentException>(() => smartMeter.RemoveMetadata(new MetadataId(Guid.NewGuid())));
    }
}