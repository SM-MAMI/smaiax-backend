using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Specifications;

namespace SMAIAXBackend.Domain.UnitTests.Specifications;

[TestFixture]
public class PolicyMatchesRequestSpecificationTests
{
    [Test]
    public void Given_PolicyMatchesRequestCriteria_When_IsSatisfiedByIsCalled_Then_ReturnsTrue()
    {
        // Given
        var policyFilter = new PolicyFilter(MeasurementResolution.Hour, 1, 4, [], LocationResolution.Country, 100);
        var policyRequest = PolicyRequest.Create(new PolicyRequestId(Guid.NewGuid()), false, policyFilter);
        var specification = new PolicyMatchesRequestSpecification(policyRequest);
        var policy = Policy.Create(new PolicyId(Guid.NewGuid()), "policy1", MeasurementResolution.Hour, LocationResolution.Country,
            90, new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(policy);

        // Then
        Assert.That(result, Is.True);
    }

    [Test]
    public void Given_PolicyDoesNotMatchPriceCriteria_When_IsSatisfiedByIsCalled_Then_ReturnsFalse()
    {
        // Given
        var policyFilter = new PolicyFilter(MeasurementResolution.Hour, 1, 4, [], LocationResolution.Country, 100);
        var policyRequest = PolicyRequest.Create(new PolicyRequestId(Guid.NewGuid()), false, policyFilter);
        var specification = new PolicyMatchesRequestSpecification(policyRequest);
        var policy = Policy.Create(new PolicyId(Guid.NewGuid()), "policy1", MeasurementResolution.Hour, LocationResolution.Country,
            110, new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(policy);

        // Then
        Assert.That(result, Is.False);
    }

    [Test]
    public void Given_PolicyDoesNotMatchResolutionCriteria_When_IsSatisfiedByIsCalled_Then_ReturnsFalse()
    {
        // Given
        var policyFilter = new PolicyFilter(MeasurementResolution.Hour, 1, 4, [], LocationResolution.Country, 100);
        var policyRequest = PolicyRequest.Create(new PolicyRequestId(Guid.NewGuid()), false, policyFilter);
        var specification = new PolicyMatchesRequestSpecification(policyRequest);
        var policy = Policy.Create(new PolicyId(Guid.NewGuid()), "policy1", MeasurementResolution.Day, LocationResolution.Country,
            90, new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(policy);

        // Then
        Assert.That(result, Is.False);
    }
}