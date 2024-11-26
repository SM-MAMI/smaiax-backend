using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.UnitTests.Model;

[TestFixture]
public class PolicyRequestTests
{
    [Test]
    public void GivenPolicyRequestDetails_WhenCreatePolicyRequest_ThenDetailsEquals()
    {
        // Given
        var policyRequestId = new PolicyRequestId(Guid.NewGuid());
        var isAutomaticContractingEnabled = true;
        var policyFilter = new PolicyFilter(MeasurementResolution.Raw, 1, 5, [],
            LocationResolution.City, 100);

        // When
        var policyRequest = PolicyRequest.Create(policyRequestId, isAutomaticContractingEnabled, policyFilter);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(policyRequest.Id, Is.EqualTo(policyRequestId));
            Assert.That(policyRequest.IsAutomaticContractingEnabled, Is.EqualTo(isAutomaticContractingEnabled));
            Assert.That(policyRequest.PolicyFilter, Is.EqualTo(policyFilter));
            Assert.That(policyRequest.State, Is.EqualTo(PolicyRequestState.Pending));
        });
    }
}