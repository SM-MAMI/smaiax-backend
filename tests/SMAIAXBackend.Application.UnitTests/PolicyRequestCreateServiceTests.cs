using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class PolicyRequestCreateServiceTests
{
    private Mock<IPolicyRequestRepository> _policyRequestRepositoryMock;
    private Mock<IUserValidationService> _userValidationServiceMock;
    private PolicyRequestCreateService _policyRequestCreateService;

    [SetUp]
    public void Setup()
    {
        _policyRequestRepositoryMock = new Mock<IPolicyRequestRepository>();
        _userValidationServiceMock = new Mock<IUserValidationService>();
        _policyRequestCreateService =
            new PolicyRequestCreateService(_policyRequestRepositoryMock.Object, _userValidationServiceMock.Object);
    }

    [Test]
    public async Task GivenPolicyRequestCreateDtoAndExistentUserId_WhenCreatePolicyRequest_ThenPolicyRequestIdIsReturned()
    {
        // Given
        var policyRequestIdExpected = new PolicyRequestId(Guid.NewGuid());
        var userIdExpected = new UserId(Guid.NewGuid());
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

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userIdExpected.Id.ToString()))
            .ReturnsAsync(userIdExpected);
        _policyRequestRepositoryMock.Setup(repo => repo.NextIdentity()).Returns(policyRequestIdExpected);

        // When
        var policyRequestIdActual = await _policyRequestCreateService.CreatePolicyRequestAsync(policyRequestCreateDto,
            userIdExpected.Id.ToString());

        // Then
        Assert.That(policyRequestIdActual, Is.EqualTo(policyRequestIdExpected.Id));
        _policyRequestRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<PolicyRequest>()), Times.Once);
    }
}