using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<UserManager<IdentityUser>> _userManagerMock;
    private Mock<ILogger<UserService>> _loggerMock;
    private IUserService _userService;

    [OneTimeSetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null
        );
        _loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_userRepositoryMock.Object, _userManagerMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GivenValidRegisterDto_WhenRegistrationSucceeds_ThenUserIsAddedToRepository()
    {
        // Given
        var registerDto = new RegisterDto("test@example.com", "Password123!", new Name("John", "Doe"),
            new Address("123 Main St", "Anytown", "NY", "12345", "USA"));

        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userRepositoryMock
            .Setup(repo => repo.NextIdentity())
            .Returns(new UserId(Guid.NewGuid()));

        // When
        await _userService.RegisterAsync(registerDto);

        // Then
        _userManagerMock.Verify(
            um => um.CreateAsync(It.Is<IdentityUser>(iu => iu.Email == registerDto.Email), registerDto.Password),
            Times.Once);
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
    }
    
    [Test]
    public void GivenInvalidRegisterDto_WhenUserCreationFails_ThenRegistrationExceptionIsThrown()
    {
        // Given
        var registerDto = new RegisterDto("test@example.com", "WeakPassword123!", new Name("John", "Doe"),
            new Address("123 Main St", "Anytown", "NY", "12345", "USA"));

        var identityErrors = new List<IdentityError> { new() { Description = "Password is too weak" } };
        var identityResult = IdentityResult.Failed(identityErrors.ToArray());

        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(identityResult);
        
        _userRepositoryMock
            .Setup(repo => repo.NextIdentity())
            .Returns(new UserId(Guid.NewGuid()));

        // When
        var exception = Assert.ThrowsAsync<RegistrationException>(() => _userService.RegisterAsync(registerDto));

        // Then
        Assert.That(exception.Message, Does.Contain("Password is too weak"));
        _userManagerMock.Verify(
            um => um.CreateAsync(It.Is<IdentityUser>(iu => iu.Email == registerDto.Email), registerDto.Password),
            Times.Once);
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
    }
}