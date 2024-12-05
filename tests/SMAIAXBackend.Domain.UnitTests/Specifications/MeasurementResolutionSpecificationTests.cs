using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Specifications;

namespace SMAIAXBackend.Domain.UnitTests.Specifications;

[TestFixture]
public class MeasurementResolutionSpecificationTests
{
    [Test]
    public void Given_MeasurementResolutionIsEqual_When_IsSatisfiedByIsCalled_Then_ReturnsTrue()
    {
        // Given
        var specification = new MeasurementResolutionSpecification(MeasurementResolution.Day);
        var policy = Policy.Create(new PolicyId(Guid.NewGuid()), "policy1", MeasurementResolution.Day, LocationResolution.Country,
            100, new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(policy);

        // Then
        Assert.That(result, Is.True);
    }

    [Test]
    public void Given_MeasurementResolutionIsLower_When_IsSatisfiedByIsCalled_Then_ReturnsTrue()
    {
        // Given
        var specification = new MeasurementResolutionSpecification(MeasurementResolution.Day);
        var policy = Policy.Create(new PolicyId(Guid.NewGuid()), "policy1", MeasurementResolution.Hour, LocationResolution.Country,
            100, new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(policy);

        // Then
        Assert.That(result, Is.True);
    }

    [Test]
    public void Given_MeasurementResolutionIsHigher_When_IsSatisfiedByIsCalled_Then_ReturnsFalse()
    {
        // Given
        var specification = new MeasurementResolutionSpecification(MeasurementResolution.Hour);
        var policy = Policy.Create(new PolicyId(Guid.NewGuid()), "policy1", MeasurementResolution.Day, LocationResolution.Country,
            100, new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(policy);

        // Then
        Assert.That(result, Is.False);
    }
}