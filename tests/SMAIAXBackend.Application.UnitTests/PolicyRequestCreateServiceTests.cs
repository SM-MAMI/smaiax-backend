using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class PolicyRequestCreateServiceTests
{
    private Mock<IPolicyRequestRepository> _policyRequestRepositoryMock;
    private Mock<IPolicyMatchingService> _policyMatchingServiceMock;
    private PolicyRequestCreateService _policyRequestCreateService;

    [SetUp]
    public void Setup()
    {
        _policyRequestRepositoryMock = new Mock<IPolicyRequestRepository>();
        _policyMatchingServiceMock = new Mock<IPolicyMatchingService>();
        _policyRequestCreateService =
            new PolicyRequestCreateService(_policyRequestRepositoryMock.Object, _policyMatchingServiceMock.Object);
    }

    [Test]
    public async Task GivenPolicyRequestCreateDtoAndExistentUserId_WhenCreatePolicyRequest_ThenMatchingPoliciesAreReturned()
    {
        // Given
        var policyRequestIdExpected = new PolicyRequestId(Guid.NewGuid());
        var policyRequestCreateDto = new PolicyRequestCreateDto(
            isAutomaticContractingEnabled: true,
            measurementResolution: MeasurementResolution.Hour,
            minHouseHoldSize: 1,
            maxHouseHoldSize: 5,
            locations:
            [
                new LocationDto(
                    streetName: "Test Street",
                    city: "Test City",
                    state: "Test State",
                    country: "Test Country",
                    continent: Continent.Africa
                )
            ],
            locationResolution: LocationResolution.State,
            maxPrice: 100);

        var policies = new List<PolicyDto>
        {
            new(Guid.NewGuid(), MeasurementResolution.Hour, LocationResolution.State, 50),
            new(Guid.NewGuid(), MeasurementResolution.Hour, LocationResolution.State, 150),
        };

        _policyRequestRepositoryMock.Setup(repo => repo.NextIdentity()).Returns(policyRequestIdExpected);
        _policyMatchingServiceMock.Setup(service => service.GetMatchingPoliciesAsync(policyRequestIdExpected.Id))
            .ReturnsAsync(policies);

        // When
        var matchingPolicies = await _policyRequestCreateService.CreatePolicyRequestAsync(policyRequestCreateDto);

        // Then
        Assert.That(matchingPolicies, Is.Not.Empty);
    }
}