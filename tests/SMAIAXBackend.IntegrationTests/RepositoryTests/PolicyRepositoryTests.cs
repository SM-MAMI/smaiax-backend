using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.IntegrationTests.RepositoryTests;

[TestFixture]
public class PolicyRepositoryTests : TestBase
{
    [Test]
    public async Task GivenPolicy_WhenAdd_ThenExpectedPolicyIsPersisted()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39"));
        var policyExpected = Policy.Create(new PolicyId(Guid.NewGuid()), MeasurementResolution.Hour,
            LocationResolution.State, 1000, smartMeterId);

        // When
        await _policyRepository.AddAsync(policyExpected);
        var policyActual = await _tenant1DbContext.Policies
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id.Equals(policyExpected.Id));

        // Then
        Assert.That(policyActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(policyActual.Id, Is.EqualTo(policyExpected.Id));
            Assert.That(policyActual.MeasurementResolution, Is.EqualTo(policyExpected.MeasurementResolution));
            Assert.That(policyActual.LocationResolution, Is.EqualTo(policyExpected.LocationResolution));
            Assert.That(policyActual.Price, Is.EqualTo(policyExpected.Price));
            Assert.That(policyActual.State, Is.EqualTo(policyExpected.State));
            Assert.That(policyActual.SmartMeterId, Is.EqualTo(policyExpected.SmartMeterId));
        });
    }
}