using System.Globalization;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.Domain.UnitTests;

[TestFixture]
public class PolicyRequestTests
{
    [Test]
    public void GivenPolicyRequest_WhenDeepClone_ThenPropertiesAreEqualButObjectsAreNotTheSame()
    {
        // Given
        var policyFilter = new PolicyFilter(MeasurementResolution.Hour,
            [5],
            [new Location("Some street", "Some city", "Some state", new RegionInfo("de-AT"), Continent.Europe)],
            LocationResolution.Country, 200);
        var originalPolicyRequest = PolicyRequest.Create(new PolicyRequestId(Guid.NewGuid()), false, policyFilter);

        // When
        var copiedPolicyRequest = PolicyRequest.DeepClone(originalPolicyRequest);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(copiedPolicyRequest.Id, Is.EqualTo(originalPolicyRequest.Id));
            Assert.That(copiedPolicyRequest.IsAutomaticContractingEnabled,
                Is.EqualTo(originalPolicyRequest.IsAutomaticContractingEnabled));
            Assert.That(copiedPolicyRequest.PolicyFilter.MeasurementResolution,
                Is.EqualTo(originalPolicyRequest.PolicyFilter.MeasurementResolution));
            Assert.That(copiedPolicyRequest.PolicyFilter.HouseHoldSizes,
                Is.EqualTo(originalPolicyRequest.PolicyFilter.HouseHoldSizes));
            Assert.That(copiedPolicyRequest.PolicyFilter.Locations,
                Is.EqualTo(originalPolicyRequest.PolicyFilter.Locations));
            Assert.That(copiedPolicyRequest.PolicyFilter.LocationResolution,
                Is.EqualTo(originalPolicyRequest.PolicyFilter.LocationResolution));
            Assert.That(copiedPolicyRequest.PolicyFilter.MaxPrice,
                Is.EqualTo(originalPolicyRequest.PolicyFilter.MaxPrice));
            Assert.That(copiedPolicyRequest, Is.Not.SameAs(originalPolicyRequest));
            Assert.That(copiedPolicyRequest.Id, Is.Not.SameAs(originalPolicyRequest.Id));
            Assert.That(copiedPolicyRequest.PolicyFilter, Is.Not.SameAs(originalPolicyRequest.PolicyFilter));
            Assert.That(copiedPolicyRequest.PolicyFilter.HouseHoldSizes,
                Is.Not.SameAs(originalPolicyRequest.PolicyFilter.HouseHoldSizes));
            Assert.That(copiedPolicyRequest.PolicyFilter.Locations,
                Is.Not.SameAs(originalPolicyRequest.PolicyFilter.Locations));
        });
    }
}