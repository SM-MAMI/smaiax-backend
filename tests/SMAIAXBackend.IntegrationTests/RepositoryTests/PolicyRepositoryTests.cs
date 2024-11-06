using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.IntegrationTests.RepositoryTests;

[TestFixture]
public class PolicyRepositoryTests : TestBase
{
    [Test]
    public async Task GivenPolicy_WhenAdd_ThenExpectedPolicyIsPersisted()
    {
        // Given
        var location = new Location("Some street name", "Some city", "Some state", "Some country",
            Continent.Antarctica);
        var userId = new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7"));
        var policyExpected = Policy.Create(new PolicyId(Guid.NewGuid()), MeasurementResolution.Hour, 4, location,
            LocationResolution.State, 1000, userId);

        // When
        await _policyRepository.AddAsync(policyExpected);
        var policyActual = await _applicationDbContext.Policies
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id.Equals(policyExpected.Id));

        // Then
        Assert.That(policyActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(policyActual.Id, Is.EqualTo(policyExpected.Id));
            Assert.That(policyActual.MeasurementResolution, Is.EqualTo(policyExpected.MeasurementResolution));
            Assert.That(policyActual.HouseholdSize, Is.EqualTo(policyExpected.HouseholdSize));
            Assert.That(policyActual.Location, Is.EqualTo(policyExpected.Location));
            Assert.That(policyActual.LocationResolution, Is.EqualTo(policyExpected.LocationResolution));
            Assert.That(policyActual.Price, Is.EqualTo(policyExpected.Price));
            Assert.That(policyActual.State, Is.EqualTo(policyExpected.State));
            Assert.That(policyActual.UserId, Is.EqualTo(policyExpected.UserId));
        });
    }
}