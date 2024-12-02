using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Specifications;

namespace SMAIAXBackend.Domain.UnitTests.Specifications;

[TestFixture]
public class PriceSpecificationTests
{
    [Test]
    public void Given_PriceIsEqualToMaxPrice_When_IsSatisfiedByIsCalled_Then_ReturnsTrue()
    {
        // Given
        var specification = new PriceSpecification(100);
        var policy = Policy.Create(new PolicyId(Guid.NewGuid()), "policy1", MeasurementResolution.Day, LocationResolution.Country,
            100, new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(policy);

        // Then
        Assert.That(result, Is.True);
    }

    [Test]
    public void Given_PriceIsLessThanMaxPrice_When_IsSatisfiedByIsCalled_Then_ReturnsTrue()
    {
        // Given
        var specification = new PriceSpecification(100);
        var policy = Policy.Create(new PolicyId(Guid.NewGuid()), "policy1", MeasurementResolution.Day, LocationResolution.Country,
            90, new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(policy);

        // Then
        Assert.That(result, Is.True);
    }

    [Test]
    public void Given_PriceIsGreaterThanMaxPrice_When_IsSatisfiedByIsCalled_Then_ReturnsFalse()
    {
        // Given
        var specification = new PriceSpecification(100);
        var policy = Policy.Create(new PolicyId(Guid.NewGuid()), "policy1", MeasurementResolution.Day, LocationResolution.Country,
            110, new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(policy);

        // Then
        Assert.That(result, Is.False);
    }
}