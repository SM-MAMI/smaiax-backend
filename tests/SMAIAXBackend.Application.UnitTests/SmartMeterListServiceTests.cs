using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterListServiceTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<IPolicyRepository> _policyRepositoryMock;
    private Mock<IUserValidationService> _userValidationServiceMock;
    private Mock<ILogger<SmartMeterListService>> _loggerMock;
    private SmartMeterListService _smartMeterListService;

    [SetUp]
    public void Setup()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _policyRepositoryMock = new Mock<IPolicyRepository>();
        _userValidationServiceMock = new Mock<IUserValidationService>();
        _loggerMock = new Mock<ILogger<SmartMeterListService>>();
        _smartMeterListService =
            new SmartMeterListService(_smartMeterRepositoryMock.Object, _policyRepositoryMock.Object,
                _userValidationServiceMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GivenUserId_WhenGetSmartMetersByUserId_ThenExpectedSmartMetersAreReturned()
    {
        // Given
        var userId = new UserId(Guid.NewGuid());
        var smartMetersExpected = new List<SmartMeter>()
        {
            SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 1", userId),
            SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 2", userId)
        };

        var policies = new List<Policy>();

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userId.Id.ToString()))
            .ReturnsAsync(userId);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMetersByUserIdAsync(userId))
            .ReturnsAsync(smartMetersExpected);
        _policyRepositoryMock.Setup(repo =>
            repo.GetPoliciesBySmartMeterIdAndUserIdAsync(It.IsAny<SmartMeterId>(), userId)).ReturnsAsync(policies);

        // When
        var smartMetersActual = await _smartMeterListService.GetSmartMetersByUserIdAsync(userId.Id.ToString());

        // Then
        Assert.That(smartMetersActual, Is.Not.Null);
        Assert.That(smartMetersActual, Has.Count.EqualTo(smartMetersExpected.Count));

        for (int i = 0; i < smartMetersActual.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(smartMetersActual[i].Id, Is.EqualTo(smartMetersExpected[i].Id.Id));
                Assert.That(smartMetersActual[i].Name, Is.EqualTo(smartMetersExpected[i].Name));
                Assert.That(smartMetersActual[i].MetadataCount, Is.EqualTo(smartMetersExpected[i].Metadata.Count));
                Assert.That(smartMetersActual[i].PolicyCount, Is.EqualTo(policies.Count));
            });
        }
    }

    [Test]
    public async Task
        GivenValidSmartMeterIdAndUserId_WhenGetSmartMeterByIdAndUserIdAsync_ThenExpectedSmartMeterIsReturned()
    {
        // Given
        var smartMeterId = Guid.NewGuid();
        var userId = new UserId(Guid.NewGuid());
        var smartMeterExpected = SmartMeter.Create(new SmartMeterId(smartMeterId), "Smart Meter 1", userId);
        var policies = new List<Policy>
        {
            Policy.Create(new PolicyId(Guid.NewGuid()), MeasurementResolution.Day, LocationResolution.City, 1000,
                userId, smartMeterExpected.Id)
        };

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userId.Id.ToString()))
            .ReturnsAsync(userId);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAndUserIdAsync(smartMeterExpected.Id, userId))
            .ReturnsAsync(smartMeterExpected);
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesBySmartMeterIdAndUserIdAsync(smartMeterExpected.Id, userId))
            .ReturnsAsync(policies);

        // When
        var smartMeterActual =
            await _smartMeterListService.GetSmartMeterByIdAndUserIdAsync(smartMeterId, userId.Id.ToString());

        // Then
        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(smartMeterActual.Id, Is.EqualTo(smartMeterExpected.Id.Id));
            Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterExpected.Name));
            Assert.That(smartMeterActual.MetadataCount, Is.EqualTo(smartMeterExpected.Metadata.Count));
            Assert.That(smartMeterActual.PolicyCount, Is.EqualTo(policies.Count));
        });
    }

    [Test]
    public void
        GivenNonExistentSmartMeterIdOrUserId_WhenGetSmartMeterByIdAndUserIdAsync_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var userId = new UserId(Guid.NewGuid());

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userId.Id.ToString()))
            .ReturnsAsync(userId);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAndUserIdAsync(smartMeterId, userId))
            .ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(async () =>
            await _smartMeterListService.GetSmartMeterByIdAndUserIdAsync(smartMeterId.Id, userId.Id.ToString())
        );
    }
}