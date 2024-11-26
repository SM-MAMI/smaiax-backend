using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.UnitTests.Model;

[TestFixture]
public class PolicyTests
{
    [Test]
    public void GivenPolicyDetails_WhenCreatePolicy_ThenDetailsEquals()
    {
        // Given
        var policyId = new PolicyId(Guid.NewGuid());
        const MeasurementResolution measurementResolution = MeasurementResolution.Raw;
        const LocationResolution locationResolution = LocationResolution.City;
        const decimal price = 100.0m;
        var smartMeterId = new SmartMeterId(Guid.NewGuid());

        // When
        var policy = Policy.Create(policyId, measurementResolution, locationResolution, price, smartMeterId);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(policy.Id, Is.EqualTo(policyId));
            Assert.That(policy.MeasurementResolution, Is.EqualTo(measurementResolution));
            Assert.That(policy.LocationResolution, Is.EqualTo(locationResolution));
            Assert.That(policy.Price, Is.EqualTo(price));
            Assert.That(policy.State, Is.EqualTo(PolicyState.Active));
            Assert.That(policy.SmartMeterId, Is.EqualTo(smartMeterId));
        });
    }
}