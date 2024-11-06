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
public class PolicyCreateServiceTests
{
    private Mock<IUserValidationService> _userValidationServiceMock;
    private Mock<IPolicyRepository> _policyRepositoryMock;
    private PolicyCreateService _policyCreateService;

    [SetUp]
    public void Setup()
    {
        _userValidationServiceMock = new Mock<IUserValidationService>();
        _policyRepositoryMock = new Mock<IPolicyRepository>();
        _policyCreateService = new PolicyCreateService(_policyRepositoryMock.Object, _userValidationServiceMock.Object);
    }

    [Test]
    public async Task GivenPolicyCreateDtoAndExistentUserId_WhenCreatePolicy_ThenPolicyIdIsReturned()
    {
        // Given
        var policyIdExpected = new PolicyId(Guid.NewGuid());
        var policyCreateDto = new PolicyCreateDto(MeasurementResolution.Hour, 4,
            new LocationDto("Test Street", "Test City", "Test State", "Test Country", Continent.Africa),
            LocationResolution.City, 100);
        var userId = new UserId(Guid.NewGuid());

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userId.Id.ToString()))
            .ReturnsAsync(userId);
        _policyRepositoryMock.Setup(repo => repo.NextIdentity()).Returns(policyIdExpected);

        // When
        var policyIdActual = await _policyCreateService.CreatePolicyAsync(policyCreateDto, userId.Id.ToString());

        // Then
        Assert.That(policyIdActual, Is.EqualTo(policyIdExpected.Id));
        _policyRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Policy>()), Times.Once);
    }
}