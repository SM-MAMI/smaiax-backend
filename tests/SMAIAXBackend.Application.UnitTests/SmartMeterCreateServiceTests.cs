using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterCreateServiceTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<ILogger<SmartMeterCreateService>> _loggerMock;
    private SmartMeterCreateService _smartMeterCreateService;

    [SetUp]
    public void Setup()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<SmartMeterCreateService>>();
        _smartMeterCreateService = new SmartMeterCreateService(_smartMeterRepositoryMock.Object,
            _userRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GivenSmartMeterCreateDtoAndExistentUserId_WhenAddSmartMeter_ThenSmartMeterIdIsReturned()
    {
        // Given
        var smartMeterIdExpected = new SmartMeterId(Guid.NewGuid());
        var smartMeterCreateDto = new SmartMeterCreateDto("Test Smart Meter");
        var userId = new UserId(Guid.NewGuid());
        var user = User.Create(userId, new Name("John", "Doe"), "john.doe@example.com");

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _smartMeterRepositoryMock.Setup(repo => repo.NextIdentity()).Returns(smartMeterIdExpected);

        // When
        var smartMeterIdActual = await _smartMeterCreateService.AddSmartMeterAsync(smartMeterCreateDto, userId.Id.ToString());

        // Then
        Assert.That(smartMeterIdActual, Is.EqualTo(smartMeterIdExpected.Id));
        _smartMeterRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<SmartMeter>()), Times.Once);
    }

    [Test]
    public void GivenSmartMeterCreateDtoAndNoUserId_WhenAddSmartMeter_ThenInvalidTokenExceptionIsThrown()
    {
        // Given
        var smartMeterCreateDto = new SmartMeterCreateDto("Test Smart Meter");

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () =>
             await _smartMeterCreateService.AddSmartMeterAsync(smartMeterCreateDto, null));
    }

    [Test]
    public void GivenSmartMeterCreateDtoAndInvalidUserId_WhenAddSmartMeter_ThenInvalidTokenExceptionIsThrown()
    {
        // Given
        var smartMeterCreateDto = new SmartMeterCreateDto("Test Smart Meter");
        const string userId = "42";

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () =>
            await _smartMeterCreateService.AddSmartMeterAsync(smartMeterCreateDto, userId));
    }

    [Test]
    public void GivenSmartMeterCreateDtoAndNonExistentUserId_WhenAddSmartMeter_ThenUserNotFoundExceptionIsThrown()
    {
        // Given
        var smartMeterCreateDto = new SmartMeterCreateDto("Test Smart Meter");
        var userId = new UserId(Guid.NewGuid());

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);

        // When ... Then
        Assert.ThrowsAsync<UserNotFoundException>(async () =>
            await _smartMeterCreateService.AddSmartMeterAsync(smartMeterCreateDto, userId.Id.ToString()));
    }
}