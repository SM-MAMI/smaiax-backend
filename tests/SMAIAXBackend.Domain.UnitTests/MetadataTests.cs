using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.UnitTests;

[TestFixture]
public class MetadataTests
{
    [Test]
    public void GivenMetadataDetails_WhenCreateMetadata_ThenDetailsEquals()
    {
        // Given
        var metadataId = new MetadataId(Guid.NewGuid());
        var validFrom = DateTime.UtcNow;
        var location = new Location("StreetName", "City", "State", "Country", Continent.Europe);
        const int householdSize = 1;
        var smartMeterId = new SmartMeterId(Guid.NewGuid());

        // When
        var metadata = Metadata.Create(metadataId, validFrom, location, householdSize, smartMeterId);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(metadata.Id, Is.EqualTo(metadataId));
            Assert.That(metadata.ValidFrom, Is.EqualTo(validFrom));
            Assert.That(metadata.Location, Is.EqualTo(location));
            Assert.That(metadata.HouseholdSize, Is.EqualTo(householdSize));
            Assert.That(metadata.SmartMeterId, Is.EqualTo(smartMeterId));
        });
    }
}