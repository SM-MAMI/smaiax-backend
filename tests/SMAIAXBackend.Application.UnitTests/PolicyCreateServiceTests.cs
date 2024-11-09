using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class PolicyCreateServiceTests
{
    private Mock<IUserValidationService> _userValidationServiceMock;
    private Mock<IPolicyRepository> _policyRepositoryMock;
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<ILogger<PolicyCreateService>> _loggerMock;
    private PolicyCreateService _policyCreateService;

    [SetUp]
    public void Setup()
    {
        _userValidationServiceMock = new Mock<IUserValidationService>();
        _policyRepositoryMock = new Mock<IPolicyRepository>();
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _loggerMock = new Mock<ILogger<PolicyCreateService>>();
        _policyCreateService = new PolicyCreateService(_policyRepositoryMock.Object, _smartMeterRepositoryMock.Object,
            _userValidationServiceMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GivenPolicyCreateDtoAndExistentUserId_WhenCreatePolicy_ThenPolicyIdIsReturned()
    {
        // Given
        var policyIdExpected = new PolicyId(Guid.NewGuid());
        var userIdExpected = new UserId(Guid.NewGuid());
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 1", userIdExpected);
        var policyCreateDto =
            new PolicyCreateDto(MeasurementResolution.Hour, LocationResolution.None, 100, smartMeter.Id.Id);

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userIdExpected.Id.ToString()))
            .ReturnsAsync(userIdExpected);
        _policyRepositoryMock.Setup(repo => repo.NextIdentity()).Returns(policyIdExpected);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAndUserIdAsync(smartMeter.Id, userIdExpected))
            .ReturnsAsync(smartMeter);

        // When
        var policyIdActual =
            await _policyCreateService.CreatePolicyAsync(policyCreateDto, userIdExpected.Id.ToString());

        // Then
        Assert.That(policyIdActual, Is.EqualTo(policyIdExpected.Id));
        _policyRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Policy>()), Times.Once);
    }

    [Test]
    public void
        GivenPolicyCreateDtoAndNonExistentSmartMeterId_WhenCreatePolicy_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var userIdExpected = new UserId(Guid.NewGuid());
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var policyCreateDto =
            new PolicyCreateDto(MeasurementResolution.Hour, LocationResolution.City, 100, smartMeterId.Id);

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userIdExpected.Id.ToString()))
            .ReturnsAsync(userIdExpected);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAndUserIdAsync(smartMeterId, userIdExpected))
            .ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(() =>
            _policyCreateService.CreatePolicyAsync(policyCreateDto, userIdExpected.Id.ToString()));
    }

    [Test]
    public void
        GivenPolicyCreateDtoAndNonMatchingSmartMeteridAndUserId_WhenCreatePolicy_ThenSmartMeterOwnershipExceptionIsThrown()
    {
        // Given
        var userIdExpected = new UserId(Guid.NewGuid());
        var smartMeter =
            SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 1", new UserId(Guid.NewGuid()));
        var policyCreateDto =
            new PolicyCreateDto(MeasurementResolution.Hour, LocationResolution.City, 100, smartMeter.Id.Id);

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userIdExpected.Id.ToString()))
            .ReturnsAsync(userIdExpected);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAndUserIdAsync(smartMeter.Id, userIdExpected))
            .ReturnsAsync(smartMeter);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterOwnershipException>(() =>
            _policyCreateService.CreatePolicyAsync(policyCreateDto, userIdExpected.Id.ToString()));
    }

    [Test]
    public void
        GivenPolicyCreateDtoAndExistentUserIdAndNoMetadata_WhenCreatePolicy_ThenInsufficientLocationDataExceptionIsThrown()
    {
        // Given
        var policyIdExpected = new PolicyId(Guid.NewGuid());
        var userIdExpected = new UserId(Guid.NewGuid());
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 1", userIdExpected);
        var policyCreateDto =
            new PolicyCreateDto(MeasurementResolution.Hour, LocationResolution.City, 100, smartMeter.Id.Id);

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userIdExpected.Id.ToString()))
            .ReturnsAsync(userIdExpected);
        _policyRepositoryMock.Setup(repo => repo.NextIdentity()).Returns(policyIdExpected);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAndUserIdAsync(smartMeter.Id, userIdExpected))
            .ReturnsAsync(smartMeter);

        // When ... Then
        Assert.ThrowsAsync<InsufficientLocationDataException>(() =>
            _policyCreateService.CreatePolicyAsync(policyCreateDto, userIdExpected.Id.ToString()));
    }

    [Test]
    public void
        GivenPolicyCreateDtoAndExistentUserIdAndInsufficientLocation_WhenCreatePolicy_ThenInsufficientLocationDataExceptionIsThrown()
    {
        // Given
        var policyIdExpected = new PolicyId(Guid.NewGuid());
        var userIdExpected = new UserId(Guid.NewGuid());
        var smartMeter = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 1", userIdExpected);
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location(null, null, "Some State", null, null), 4, smartMeter.Id);
        smartMeter.AddMetadata(metadata);
        var policyCreateDto =
            new PolicyCreateDto(MeasurementResolution.Hour, LocationResolution.City, 100, smartMeter.Id.Id);

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userIdExpected.Id.ToString()))
            .ReturnsAsync(userIdExpected);
        _policyRepositoryMock.Setup(repo => repo.NextIdentity()).Returns(policyIdExpected);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAndUserIdAsync(smartMeter.Id, userIdExpected))
            .ReturnsAsync(smartMeter);

        // When ... Then
        Assert.ThrowsAsync<InsufficientLocationDataException>(() =>
            _policyCreateService.CreatePolicyAsync(policyCreateDto, userIdExpected.Id.ToString()));
    }
}