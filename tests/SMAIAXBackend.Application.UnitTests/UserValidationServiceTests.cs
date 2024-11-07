using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class UserValidationServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<ILogger<UserValidationService>> _loggerMock;
    private UserValidationService _userValidationService;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserValidationService>>();
        _userValidationService = new UserValidationService(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GivenUserId_WhenValidateUser_ThenUserIdIsReturned()
    {
        // Given
        var tenantId = new TenantId(Guid.NewGuid());
        var userIdExpected = new UserId(Guid.NewGuid());
        var user = User.Create(userIdExpected, new Name("John", "Doe"), "johndoe", "john.doe@example.com", tenantId);

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userIdExpected)).ReturnsAsync(user);

        // When
        var userIdActual = await _userValidationService.ValidateUserAsync(userIdExpected.Id.ToString());

        // Then
        Assert.That(userIdActual, Is.EqualTo(user));
    }

    [Test]
    public void GivenSmartMeterCreateDtoAndNoUserId_WhenAddSmartMeter_ThenInvalidTokenExceptionIsThrown()
    {
        // Given
        string? userId = null;

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () =>
            await _userValidationService.ValidateUserAsync(userId));
    }

    [Test]
    public void GivenSmartMeterCreateDtoAndInvalidUserId_WhenAddSmartMeter_ThenInvalidTokenExceptionIsThrown()
    {
        // Given
        const string userId = "42";

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () =>
            await _userValidationService.ValidateUserAsync(userId));
    }

    [Test]
    public void GivenSmartMeterCreateDtoAndNonExistentUserId_WhenAddSmartMeter_ThenUserNotFoundExceptionIsThrown()
    {
        // Given
        var userId = new UserId(Guid.NewGuid());

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);

        // When ... Then
        Assert.ThrowsAsync<UserNotFoundException>(async () =>
            await _userValidationService.ValidateUserAsync(userId.Id.ToString()));
    }
}