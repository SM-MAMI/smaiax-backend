using System.Globalization;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Domain.UnitTests;

[TestFixture]
public class PolicyTests
{
    [Test]
    public void GivenPolicy_WhenDeepClone_ThenPropertiesAreEqualButObjectsAreNotTheSame()
    {
        // Given
        var originalPolicy = Policy.Create(new PolicyId(Guid.NewGuid()), MeasurementResolution.Hour, 5,
            new Location("Some street", "Some city", "Some state", new RegionInfo("de-AT"), Continent.Europe),
            LocationResolution.Country, 200);


        // When
        var copiedPolicy = Policy.DeepClone(originalPolicy);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(copiedPolicy.Id, Is.EqualTo(originalPolicy.Id));
            Assert.That(copiedPolicy.MeasurementResolution, Is.EqualTo(originalPolicy.MeasurementResolution));
            Assert.That(copiedPolicy.HouseholdSize, Is.EqualTo(originalPolicy.HouseholdSize));
            Assert.That(copiedPolicy.Location, Is.EqualTo(originalPolicy.Location));
            Assert.That(copiedPolicy.LocationResolution, Is.EqualTo(originalPolicy.LocationResolution));
            Assert.That(copiedPolicy.Price, Is.EqualTo(originalPolicy.Price));
            Assert.That(copiedPolicy, Is.Not.SameAs(originalPolicy));
            Assert.That(copiedPolicy.Id, Is.Not.SameAs(originalPolicy.Id));
            Assert.That(copiedPolicy.Location, Is.Not.SameAs(originalPolicy.Location));
        });
    }
}