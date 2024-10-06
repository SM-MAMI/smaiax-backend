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
            .Setup(ts => ts.GenerateRefreshTokenAsync(expectedTokenId.ToString(), user.Id))
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

    [Test]
    public async Task GivenValidTokens_WhenRefreshingTokens_ThenNewTokensAreReturned()
    {
        // Given
        var userId = new UserId(Guid.NewGuid());
        var identityUser = new IdentityUser { Id = userId.ToString(), UserName = "john.doe@example.com" };
        var validAccessToken = "validAccessToken";
        var validRefreshToken = "validRefreshToken";
        var refreshTokenId = new RefreshTokenId(Guid.NewGuid());
        var tokenDto = new TokenDto(validAccessToken, validRefreshToken);

        var existingRefreshToken = RefreshToken.Create(
            refreshTokenId,
            userId,
            Guid.NewGuid().ToString(),
            validRefreshToken,
            isValid: true,
            expiresAt: DateTime.UtcNow.AddMinutes(10)
        );

        var newRefreshTokenId = new RefreshTokenId(Guid.NewGuid());
        var newRefreshTokenString = "newRefreshToken";
        var newAccessToken = "newAccessToken";

        var newRefreshToken = RefreshToken.Create(
            newRefreshTokenId,
            userId,
            Guid.NewGuid().ToString(),
            newRefreshTokenString,
            isValid: true,
            expiresAt: DateTime.UtcNow.AddMinutes(10)
        );

        _tokenServiceMock.Setup(ts => ts.GetRefreshTokenByTokenAsync(validRefreshToken))
            .ReturnsAsync(existingRefreshToken);
        _tokenServiceMock.Setup(ts => ts.ValidateAccessToken(validAccessToken, userId, existingRefreshToken.JwtTokenId))
            .Returns(true);
        _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(identityUser);
        _tokenServiceMock.Setup(ts => ts.NextIdentity()).Returns(newRefreshTokenId.Id);
        _tokenServiceMock.Setup(ts => ts.GenerateRefreshTokenAsync(newRefreshTokenId.ToString(), userId.ToString()))
            .ReturnsAsync(newRefreshToken);
        _tokenServiceMock.Setup(ts =>
                ts.GenerateAccessTokenAsync(It.IsAny<string>(), userId.ToString(), identityUser.UserName))
            .ReturnsAsync(newAccessToken);

        // When
        var result = await _userService.RefreshTokensAsync(tokenDto);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(result.AccessToken, Is.EqualTo(newAccessToken));
            Assert.That(result.RefreshToken, Is.EqualTo(newRefreshToken.Token));
        });

        _tokenServiceMock.Verify(ts => ts.GetRefreshTokenByTokenAsync(validRefreshToken), Times.Once);
        _tokenServiceMock.Verify(
            ts => ts.ValidateAccessToken(validAccessToken, userId, existingRefreshToken.JwtTokenId), Times.Once);
        _tokenServiceMock.Verify(ts => ts.NextIdentity(), Times.Once);
        _tokenServiceMock.Verify(ts => ts.GenerateRefreshTokenAsync(newRefreshTokenId.ToString(), userId.ToString()),
            Times.Once);
        _tokenServiceMock.Verify(
            ts => ts.GenerateAccessTokenAsync(It.IsAny<string>(), userId.ToString(), identityUser.UserName),
            Times.Once);
        _userManagerMock.Verify(um => um.FindByIdAsync(userId.ToString()), Times.Once);
    }

    [Test]
    public void GivenNonExistentRefreshToken_WhenRefreshingTokens_ThenInvalidTokenExceptionIsThrown()
    {
        // Given
        var validAccessToken = "validAccessToken";
        var invalidRefreshToken = "invalidRefreshToken";
        var tokenDto = new TokenDto(validAccessToken, invalidRefreshToken);

        _tokenServiceMock.Setup(ts => ts.GetRefreshTokenByTokenAsync(invalidRefreshToken))
            .ReturnsAsync((RefreshToken)null!);

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () => await _userService.RefreshTokensAsync(tokenDto));
        _tokenServiceMock.Verify(ts => ts.GetRefreshTokenByTokenAsync(invalidRefreshToken), Times.Once);
    }

    [Test]
    public void Given_InvalidRefreshToken_When_RefreshingTokens_Then_InvalidTokenExceptionIsThrown()
    {
        // Given
        var validAccessToken = "validAccessToken";
        var invalidRefreshToken = "invalidRefreshToken";
        var tokenDto = new TokenDto(validAccessToken, invalidRefreshToken);

        var existingRefreshToken = RefreshToken.Create(
            new RefreshTokenId(Guid.NewGuid()),
            new UserId(Guid.NewGuid()),
            Guid.NewGuid().ToString(),
            invalidRefreshToken,
            isValid: false,
            expiresAt: DateTime.UtcNow.AddMinutes(10)
        );

        _tokenServiceMock.Setup(ts => ts.GetRefreshTokenByTokenAsync(invalidRefreshToken))
            .ReturnsAsync(existingRefreshToken);

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () => await _userService.RefreshTokensAsync(tokenDto));
        _tokenServiceMock.Verify(ts => ts.GetRefreshTokenByTokenAsync(invalidRefreshToken), Times.Once);
    }

    [Test]
    public void GivenExpiredRefreshToken_WhenRefreshingTokens_ThenInvalidTokenExceptionIsThrown()
    {
        // Given
        var validAccessToken = "validAccessToken";
        var expiredRefreshToken = "expiredRefreshToken";
        var tokenDto = new TokenDto(validAccessToken, expiredRefreshToken);

        var existingRefreshToken = RefreshToken.Create(
            new RefreshTokenId(Guid.NewGuid()),
            new UserId(Guid.NewGuid()),
            Guid.NewGuid().ToString(),
            expiredRefreshToken,
            isValid: true,
            expiresAt: DateTime.UtcNow.AddMinutes(-10) // Expired token
        );

        _tokenServiceMock.Setup(ts => ts.GetRefreshTokenByTokenAsync(expiredRefreshToken))
            .ReturnsAsync(existingRefreshToken);

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () => await _userService.RefreshTokensAsync(tokenDto));
        _tokenServiceMock.Verify(ts => ts.GetRefreshTokenByTokenAsync(expiredRefreshToken), Times.Once);
    }

    [Test]
    public void GivenInvalidAccessToken_WhenRefreshingTokens_ThenInvalidTokenExceptionIsThrown()
    {
        // Given
        var invalidAccessToken = "invalidAccessToken";
        var validRefreshToken = "validRefreshToken";
        var tokenDto = new TokenDto(invalidAccessToken, validRefreshToken);
        var userId = new UserId(Guid.NewGuid());
        var refreshTokenId = new RefreshTokenId(Guid.NewGuid());

        var existingRefreshToken = RefreshToken.Create(
            refreshTokenId,
            userId,
            Guid.NewGuid().ToString(),
            validRefreshToken,
            isValid: true,
            expiresAt: DateTime.UtcNow.AddMinutes(10)
        );

        _tokenServiceMock.Setup(ts => ts.GetRefreshTokenByTokenAsync(validRefreshToken))
            .ReturnsAsync(existingRefreshToken);
        _tokenServiceMock
            .Setup(ts => ts.ValidateAccessToken(invalidAccessToken, userId, existingRefreshToken.JwtTokenId))
            .Returns(false);

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () => await _userService.RefreshTokensAsync(tokenDto));
        _tokenServiceMock.Verify(ts => ts.GetRefreshTokenByTokenAsync(validRefreshToken), Times.Once);
        _tokenServiceMock.Verify(
            ts => ts.ValidateAccessToken(invalidAccessToken, userId, existingRefreshToken.JwtTokenId), Times.Once);
    }

    [Test]
    public void GivenNonExistentUserId_WhenRefreshingTokens_ThenInvalidTokenExceptionIsThrown()
    {
        // Given
        var validAccessToken = "validAccessToken";
        var validRefreshToken = "validRefreshToken";
        var tokenDto = new TokenDto(validAccessToken, validRefreshToken);
        var userId = new UserId(Guid.NewGuid());
        var refreshTokenId = new RefreshTokenId(Guid.NewGuid());

        var existingRefreshToken = RefreshToken.Create(
            refreshTokenId,
            userId,
            Guid.NewGuid().ToString(),
            validRefreshToken,
            isValid: true,
            expiresAt: DateTime.UtcNow.AddMinutes(10)
        );

        _tokenServiceMock.Setup(ts => ts.GetRefreshTokenByTokenAsync(validRefreshToken))
            .ReturnsAsync(existingRefreshToken);
        _tokenServiceMock.Setup(ts => ts.ValidateAccessToken(validAccessToken, userId, existingRefreshToken.JwtTokenId))
            .Returns(true);
        _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync((IdentityUser)null!);

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () => await _userService.RefreshTokensAsync(tokenDto));
        _tokenServiceMock.Verify(ts => ts.GetRefreshTokenByTokenAsync(validRefreshToken), Times.Once);
        _tokenServiceMock.Verify(
            ts => ts.ValidateAccessToken(validAccessToken, userId, existingRefreshToken.JwtTokenId), Times.Once);
        _userManagerMock.Verify(um => um.FindByIdAsync(userId.ToString()), Times.Once);
    }
}