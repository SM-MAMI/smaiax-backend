using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.IntegrationTests.RepositoryTests;

[TestFixture]
public class PolicyRequestRepositoryTests : TestBase
{
    [Test]
    public async Task GivenPolicyRequest_WhenAdd_ThenExpectedPolicyRequestIsPersisted()
    {
        // Given
        var policyRequestId = new PolicyRequestId(Guid.NewGuid());
        var policyFilter = new PolicyFilter(MeasurementResolution.Hour, 1, 10, [],
            LocationResolution.State, 1000);
        var policyRequestExpected = PolicyRequest.Create(policyRequestId, false, policyFilter);

        // When
        await _policyRequestRepository.AddAsync(policyRequestExpected);
        var policyRequestActual = await _tenant1DbContext.PolicyRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(pr => pr.Id.Equals(policyRequestExpected.Id));

        // Then
        Assert.That(policyRequestActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(policyRequestActual.Id, Is.EqualTo(policyRequestExpected.Id));
            Assert.That(policyRequestActual.IsAutomaticContractingEnabled,
                Is.EqualTo(policyRequestExpected.IsAutomaticContractingEnabled));
            Assert.That(policyRequestActual.PolicyFilter.MeasurementResolution,
                Is.EqualTo(policyRequestExpected.PolicyFilter.MeasurementResolution));
            Assert.That(policyRequestActual.PolicyFilter.MinHouseHoldSize,
                Is.EqualTo(policyRequestExpected.PolicyFilter.MinHouseHoldSize));
            Assert.That(policyRequestActual.PolicyFilter.MaxHouseHoldSize,
                Is.EqualTo(policyRequestExpected.PolicyFilter.MaxHouseHoldSize));
            Assert.That(policyRequestActual.PolicyFilter.Locations,
                Is.EqualTo(policyRequestExpected.PolicyFilter.Locations));
            Assert.That(policyRequestActual.PolicyFilter.LocationResolution,
                Is.EqualTo(policyRequestExpected.PolicyFilter.LocationResolution));
            Assert.That(policyRequestActual.PolicyFilter.MaxPrice,
                Is.EqualTo(policyRequestExpected.PolicyFilter.MaxPrice));
            Assert.That(policyRequestActual.State, Is.EqualTo(policyRequestExpected.State));
        });
    }

    [Test]
    public async Task GivenPolicyRequestId_WhenGetById_ThenExpectedPolicyRequestIsReturned()
    {
        // Given
        var policyRequestId = new PolicyRequestId(Guid.Parse("58af578c-9975-4633-8dfe-ff8b70b83661"));

        // When
        var policyRequestActual = await _policyRequestRepository.GetPolicyRequestByIdAsync(policyRequestId);

        // Then
        Assert.That(policyRequestActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(policyRequestActual.Id, Is.EqualTo(policyRequestId));
            Assert.That(policyRequestActual.IsAutomaticContractingEnabled, Is.False);
            Assert.That(policyRequestActual.PolicyFilter.MeasurementResolution, Is.EqualTo(MeasurementResolution.Hour));
            Assert.That(policyRequestActual.PolicyFilter.MinHouseHoldSize, Is.EqualTo(1));
            Assert.That(policyRequestActual.PolicyFilter.MaxHouseHoldSize, Is.EqualTo(10));
            Assert.That(policyRequestActual.PolicyFilter.Locations, Is.Empty);
            Assert.That(policyRequestActual.PolicyFilter.LocationResolution, Is.EqualTo(LocationResolution.State));
            Assert.That(policyRequestActual.PolicyFilter.MaxPrice, Is.EqualTo(500));
        });
    }
}