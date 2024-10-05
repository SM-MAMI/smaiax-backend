using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<ITokenService> _tokenServiceMock;
    private Mock<UserManager<IdentityUser>> _userManagerMock;
    private Mock<ILogger<UserService>> _loggerMock;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null!, null!, null!, null!, null!, null!, null!, null!
        );
        _loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_userRepositoryMock.Object, _tokenServiceMock.Object,
            _userManagerMock.Object, _loggerMock.Object);
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
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task GivenValidUsernameAndPassword_WhenLogin_ThenTokenDtoIsReturned()
    {
        // Given
        var loginDto = new LoginDto("valid@example.com", "validPassword");
        var user = new IdentityUser { Id = Guid.NewGuid().ToString(), UserName = loginDto.Username };
        var expectedTokenId = Guid.NewGuid();
        var expectedAccessToken = "accessToken123";
        var expectedRefreshToken = RefreshToken.Create(new RefreshTokenId(Guid.NewGuid()),
            new UserId(Guid.Parse(user.Id)), expectedTokenId.ToString(), "refreshToken123", true,
            DateTime.UtcNow.AddMinutes(1));

        _userManagerMock
            .Setup(um => um.FindByNameAsync(loginDto.Username))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);

        _tokenServiceMock.Setup(ts => ts.NextIdentity()).Returns(expectedTokenId);
        _tokenServiceMock
            .Setup(ts => ts.GenerateAccessTokenAsync(expectedTokenId.ToString(), user.Id, user.UserName))
            .ReturnsAsync(expectedAccessToken);
        _tokenServiceMock
            .Setup(ts => ts.GenerateRefreshToken(expectedTokenId.ToString(), user.Id))
            .ReturnsAsync(expectedRefreshToken);

        // When
        var tokenDto = await _userService.LoginAsync(loginDto);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(tokenDto.AccessToken, Is.EqualTo(expectedAccessToken));
            Assert.That(tokenDto.RefreshToken, Is.EqualTo(expectedRefreshToken.Token));
        });
        _userManagerMock.Verify(um => um.FindByNameAsync(loginDto.Username), Times.Once);
        _userManagerMock.Verify(um => um.CheckPasswordAsync(user, loginDto.Password), Times.Once);
        _tokenServiceMock.Verify(ts => ts.GenerateAccessTokenAsync(expectedTokenId.ToString(), user.Id, user.UserName),
            Times.Once);
    }

    [Test]
    public void GivenInvalidUsernameAndValidPassword_WhenLogin_ThenInvalidLoginExceptionIsThrown()
    {
        // Given
        var loginDto = new LoginDto("invalid@example.com", "validPassword");

        _userManagerMock
            .Setup(um => um.FindByNameAsync(loginDto.Username))
            .ReturnsAsync((IdentityUser)null!);

        // When
        var exception = Assert.ThrowsAsync<InvalidLoginException>(() => _userService.LoginAsync(loginDto));

        // Then
        Assert.That(exception.Message, Does.Contain("Username or password is wrong"));
        _userManagerMock.Verify(um => um.FindByNameAsync(loginDto.Username), Times.Once);
        _userManagerMock.Verify(um => um.CheckPasswordAsync(It.IsAny<IdentityUser>(), loginDto.Password), Times.Never);
        _tokenServiceMock.Verify(
            ts => ts.GenerateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GivenValidUsernameAndInvalidPassword_WhenLogin_ThenInvalidLoginExceptionIsThrown()
    {
        // Given
        var loginDto = new LoginDto("valid@example.com", "invalidPassword");
        var user = new IdentityUser { Id = "user123", UserName = loginDto.Username };

        _userManagerMock
            .Setup(um => um.FindByNameAsync(loginDto.Username))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(false);

        // When
        var exception = Assert.ThrowsAsync<InvalidLoginException>(() => _userService.LoginAsync(loginDto));

        // Then
        Assert.That(exception.Message, Does.Contain("Username or password is wrong"));
        _userManagerMock.Verify(um => um.FindByNameAsync(loginDto.Username), Times.Once);
        _userManagerMock.Verify(um => um.CheckPasswordAsync(user, loginDto.Password), Times.Once);
        _tokenServiceMock.Verify(
            ts => ts.GenerateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }
}