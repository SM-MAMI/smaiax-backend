using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.UnitTests;

[TestFixture]
public class PolicyTests
{
    [Test]
    public void GivenPolicyDetails_WhenCreatePolicy_ThenDetailsEquals()
    {
        // Given
        var policyId = new PolicyId(Guid.NewGuid());
        const MeasurementResolution measurementResolution = MeasurementResolution.Raw;
        const int householdSize = 2;
        var location = new Location("Street", "City", "State", "Country", Continent.Europe);
        const LocationResolution locationResolution = LocationResolution.City;
        const decimal price = 100.0m;
        var userId = new UserId(Guid.NewGuid());

        // When
        var policy = Policy.Create(policyId, measurementResolution, householdSize, location, locationResolution, price, userId);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(policy.Id, Is.EqualTo(policyId));
            Assert.That(policy.MeasurementResolution, Is.EqualTo(measurementResolution));
            Assert.That(policy.HouseholdSize, Is.EqualTo(householdSize));
            Assert.That(policy.Location, Is.EqualTo(location));
            Assert.That(policy.LocationResolution, Is.EqualTo(locationResolution));
            Assert.That(policy.Price, Is.EqualTo(price));
            Assert.That(policy.State, Is.EqualTo(PolicyState.Active));
            Assert.That(policy.UserId, Is.EqualTo(userId));
        });
    }
}