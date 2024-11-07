using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Domain.Repositories.Transactions;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class AuthenticationServiceTests
{
    private Mock<ITenantRepository> _tenantRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<ITokenRepository> _tokenRepositoryMock;
    private Mock<UserManager<IdentityUser>> _userManagerMock;
    private Mock<ITransactionManager> _transactionManagerMock;
    private Mock<ILogger<AuthenticationService>> _loggerMock;
    private AuthenticationService _authenticationService;

    [SetUp]
    public void Setup()
    {
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenRepositoryMock = new Mock<ITokenRepository>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null!, null!, null!, null!, null!, null!, null!, null!
        );
        _transactionManagerMock = new Mock<ITransactionManager>();
        _transactionManagerMock
            .Setup(mgr => mgr.TransactionScope(It.IsAny<Func<Task>>()))
            .Returns((Func<Task> transactionalOperation) => transactionalOperation());
        _loggerMock = new Mock<ILogger<AuthenticationService>>();
        _authenticationService = new AuthenticationService(_tenantRepositoryMock.Object, _userRepositoryMock.Object,
            _tokenRepositoryMock.Object,
            _userManagerMock.Object, _transactionManagerMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GivenValidRegisterDto_WhenRegistrationSucceeds_ThenUserIsAddedToRepository()
    {
        // Given
        var tenantId = new TenantId(Guid.NewGuid());
        var registerDto = new RegisterDto("test", "test@example.com", "Password123!", new NameDto("John", "Doe"));

        _tenantRepositoryMock
            .Setup(repo => repo.NextIdentity())
            .Returns(tenantId);

        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userRepositoryMock
            .Setup(repo => repo.NextIdentity())
            .Returns(new UserId(Guid.NewGuid()));

        // When
        await _authenticationService.RegisterAsync(registerDto);

        // Then
        _userManagerMock.Verify(
            um => um.CreateAsync(
                It.Is<IdentityUser>(iu => iu.UserName == registerDto.UserName && iu.Email == registerDto.Email),
                registerDto.Password),
            Times.Once);
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public void GivenInvalidRegisterDto_WhenUserCreationFails_ThenRegistrationExceptionIsThrown()
    {
        // Given
        var tenantId = new TenantId(Guid.NewGuid());
        var registerDto = new RegisterDto("test", "test@example.com", "WeakPassword123!", new NameDto("John", "Doe"));

        var identityErrors = new List<IdentityError> { new() { Description = "Password is too weak" } };
        var identityResult = IdentityResult.Failed(identityErrors.ToArray());

        _tenantRepositoryMock
            .Setup(repo => repo.NextIdentity())
            .Returns(tenantId);

        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(identityResult);

        _userRepositoryMock
            .Setup(repo => repo.NextIdentity())
            .Returns(new UserId(Guid.NewGuid()));

        // When
        var exception =
            Assert.ThrowsAsync<RegistrationException>(() => _authenticationService.RegisterAsync(registerDto));

        // Then
        Assert.That(exception.Message, Does.Contain("Password is too weak"));
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task GivenValidUsernameAndPassword_WhenLogin_ThenTokenDtoIsReturned()
    {
        // Given
        var loginDto = new LoginDto("valid@example.com", "validPassword");
        var user = new IdentityUser { Id = Guid.NewGuid().ToString(), UserName = loginDto.UserName };
        var expectedJwtId = Guid.NewGuid();
        var expectedRefreshTokenId = new RefreshTokenId(Guid.NewGuid());
        var expectedAccessToken = "accessToken123";
        var expectedRefreshToken = RefreshToken.Create(expectedRefreshTokenId,
            new UserId(Guid.Parse(user.Id)), expectedJwtId.ToString(), "refreshToken123", true,
            DateTime.UtcNow.AddMinutes(1));

        _userManagerMock
            .Setup(um => um.FindByNameAsync(loginDto.UserName))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);

        _tokenRepositoryMock.SetupSequence(ts => ts.NextIdentity())
            .Returns(expectedJwtId)
            .Returns(expectedRefreshTokenId.Id);

        _tokenRepositoryMock
            .Setup(ts => ts.GenerateAccessTokenAsync(expectedJwtId.ToString(), user.Id, user.UserName))
            .ReturnsAsync(expectedAccessToken);

        _tokenRepositoryMock
            .Setup(ts => ts.GenerateRefreshTokenAsync(expectedRefreshTokenId, expectedJwtId.ToString(), user.Id))
            .ReturnsAsync(expectedRefreshToken);

        // When
        var tokenDto = await _authenticationService.LoginAsync(loginDto);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(tokenDto.AccessToken, Is.EqualTo(expectedAccessToken));
            Assert.That(tokenDto.RefreshToken, Is.EqualTo(expectedRefreshToken.Token));
        });
        _userManagerMock.Verify(um => um.FindByNameAsync(loginDto.UserName), Times.Once);
        _userManagerMock.Verify(um => um.CheckPasswordAsync(user, loginDto.Password), Times.Once);
        _tokenRepositoryMock.Verify(ts => ts.GenerateAccessTokenAsync(expectedJwtId.ToString(), user.Id, user.UserName),
            Times.Once);
    }

    [Test]
    public void GivenInvalidUsernameAndValidPassword_WhenLogin_ThenInvalidLoginExceptionIsThrown()
    {
        // Given
        var loginDto = new LoginDto("invalid@example.com", "validPassword");

        _userManagerMock
            .Setup(um => um.FindByNameAsync(loginDto.UserName))
            .ReturnsAsync((IdentityUser)null!);

        // When
        var exception = Assert.ThrowsAsync<InvalidLoginException>(() => _authenticationService.LoginAsync(loginDto));

        // Then
        Assert.That(exception.Message, Does.Contain("Username or password is wrong"));
        _userManagerMock.Verify(um => um.FindByNameAsync(loginDto.UserName), Times.Once);
        _userManagerMock.Verify(um => um.CheckPasswordAsync(It.IsAny<IdentityUser>(), loginDto.Password), Times.Never);
        _tokenRepositoryMock.Verify(
            ts => ts.GenerateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GivenValidUsernameAndInvalidPassword_WhenLogin_ThenInvalidLoginExceptionIsThrown()
    {
        // Given
        var loginDto = new LoginDto("valid@example.com", "invalidPassword");
        var user = new IdentityUser { Id = "user123", UserName = loginDto.UserName };

        _userManagerMock
            .Setup(um => um.FindByNameAsync(loginDto.UserName))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(false);

        // When
        var exception = Assert.ThrowsAsync<InvalidLoginException>(() => _authenticationService.LoginAsync(loginDto));

        // Then
        Assert.That(exception.Message, Does.Contain("Username or password is wrong"));
        _userManagerMock.Verify(um => um.FindByNameAsync(loginDto.UserName), Times.Once);
        _userManagerMock.Verify(um => um.CheckPasswordAsync(user, loginDto.Password), Times.Once);
        _tokenRepositoryMock.Verify(
            ts => ts.GenerateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Test]
    public async Task GivenValidTokens_WhenRefreshingTokens_ThenNewTokensAreReturned()
    {
        // Given
        var userId = new UserId(Guid.NewGuid());
        var identityUser = new IdentityUser { Id = userId.ToString(), UserName = "john.doe@example.com" };
        const string validAccessToken = "validAccessToken";
        const string validRefreshToken = "validRefreshToken";
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
        var newJwtId = Guid.NewGuid();
        var newAccessToken = "newAccessToken";

        var newRefreshToken = RefreshToken.Create(
            newRefreshTokenId,
            userId,
            Guid.NewGuid().ToString(),
            newRefreshTokenString,
            isValid: true,
            expiresAt: DateTime.UtcNow.AddMinutes(10)
        );

        _tokenRepositoryMock.Setup(ts => ts.GetRefreshTokenByTokenAsync(validRefreshToken))
            .ReturnsAsync(existingRefreshToken);
        _tokenRepositoryMock
            .Setup(ts => ts.ValidateAccessToken(validAccessToken, userId, existingRefreshToken.JwtTokenId))
            .Returns(true);
        _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(identityUser);
        _tokenRepositoryMock.SetupSequence(ts => ts.NextIdentity())
            .Returns(newJwtId)
            .Returns(newRefreshTokenId.Id);

        _tokenRepositoryMock.Setup(ts =>
                ts.GenerateRefreshTokenAsync(newRefreshTokenId, newJwtId.ToString(), userId.ToString()))
            .ReturnsAsync(newRefreshToken);
        _tokenRepositoryMock.Setup(ts =>
                ts.GenerateAccessTokenAsync(It.IsAny<string>(), userId.ToString(), identityUser.UserName))
            .ReturnsAsync(newAccessToken);

        // When
        var result = await _authenticationService.RefreshTokensAsync(tokenDto);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(result.AccessToken, Is.EqualTo(newAccessToken));
            Assert.That(result.RefreshToken, Is.EqualTo(newRefreshToken.Token));
        });

        _tokenRepositoryMock.Verify(ts => ts.GetRefreshTokenByTokenAsync(validRefreshToken), Times.Once);
        _tokenRepositoryMock.Verify(
            ts => ts.ValidateAccessToken(validAccessToken, userId, existingRefreshToken.JwtTokenId), Times.Once);
        _tokenRepositoryMock.Verify(ts => ts.NextIdentity(), Times.Exactly(2));
        _tokenRepositoryMock.Verify(
            ts => ts.GenerateRefreshTokenAsync(newRefreshTokenId, newJwtId.ToString(), userId.ToString()),
            Times.Once);
        _tokenRepositoryMock.Verify(
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

        _tokenRepositoryMock.Setup(ts => ts.GetRefreshTokenByTokenAsync(invalidRefreshToken))
            .ReturnsAsync((RefreshToken)null!);

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () =>
            await _authenticationService.RefreshTokensAsync(tokenDto));
        _tokenRepositoryMock.Verify(ts => ts.GetRefreshTokenByTokenAsync(invalidRefreshToken), Times.Once);
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

        _tokenRepositoryMock.Setup(ts => ts.GetRefreshTokenByTokenAsync(invalidRefreshToken))
            .ReturnsAsync(existingRefreshToken);

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () =>
            await _authenticationService.RefreshTokensAsync(tokenDto));
        _tokenRepositoryMock.Verify(ts => ts.GetRefreshTokenByTokenAsync(invalidRefreshToken), Times.Once);
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

        _tokenRepositoryMock.Setup(ts => ts.GetRefreshTokenByTokenAsync(expiredRefreshToken))
            .ReturnsAsync(existingRefreshToken);

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () =>
            await _authenticationService.RefreshTokensAsync(tokenDto));
        _tokenRepositoryMock.Verify(ts => ts.GetRefreshTokenByTokenAsync(expiredRefreshToken), Times.Once);
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

        _tokenRepositoryMock.Setup(ts => ts.GetRefreshTokenByTokenAsync(validRefreshToken))
            .ReturnsAsync(existingRefreshToken);
        _tokenRepositoryMock
            .Setup(ts => ts.ValidateAccessToken(invalidAccessToken, userId, existingRefreshToken.JwtTokenId))
            .Returns(false);

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () =>
            await _authenticationService.RefreshTokensAsync(tokenDto));
        _tokenRepositoryMock.Verify(ts => ts.GetRefreshTokenByTokenAsync(validRefreshToken), Times.Once);
        _tokenRepositoryMock.Verify(
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

        _tokenRepositoryMock.Setup(ts => ts.GetRefreshTokenByTokenAsync(validRefreshToken))
            .ReturnsAsync(existingRefreshToken);
        _tokenRepositoryMock
            .Setup(ts => ts.ValidateAccessToken(validAccessToken, userId, existingRefreshToken.JwtTokenId))
            .Returns(true);
        _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync((IdentityUser)null!);

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () =>
            await _authenticationService.RefreshTokensAsync(tokenDto));
        _tokenRepositoryMock.Verify(ts => ts.GetRefreshTokenByTokenAsync(validRefreshToken), Times.Once);
        _tokenRepositoryMock.Verify(
            ts => ts.ValidateAccessToken(validAccessToken, userId, existingRefreshToken.JwtTokenId), Times.Once);
        _userManagerMock.Verify(um => um.FindByIdAsync(userId.ToString()), Times.Once);
    }

    [Test]
    public async Task GivenValidRefreshToken_WhenLogout_ThenTokenIsInvalidated()
    {
        // Given
        var refreshTokenId = new RefreshTokenId(Guid.NewGuid());
        var userId = new UserId(Guid.NewGuid());
        const string jwtTokenId = "jwt_token_id";
        const string tokenString = "valid_refresh_token";
        var token = RefreshToken.Create(
            refreshTokenId,
            userId,
            jwtTokenId,
            tokenString,
            true,
            DateTime.UtcNow.AddMinutes(10));

        _tokenRepositoryMock
            .Setup(repo => repo.GetRefreshTokenByTokenAsync(tokenString))
            .ReturnsAsync(token);
        _tokenRepositoryMock
            .Setup(repo => repo.UpdateAsync(It.Is<RefreshToken>(t => t.Token == tokenString && !t.IsValid)))
            .Returns(Task.CompletedTask);

        // When
        await _authenticationService.LogoutAsync(tokenString);

        // Then
        _tokenRepositoryMock.Verify(repo => repo.GetRefreshTokenByTokenAsync(tokenString), Times.Once);
        _tokenRepositoryMock.Verify(
            repo => repo.UpdateAsync(It.Is<RefreshToken>(t => t.Token == tokenString && !t.IsValid)), Times.Once);
    }

    [Test]
    public void GivenInvalidRefreshToken_WhenLogout_ThenUnauthorizedAccessExceptionIsThrown()
    {
        // Given
        var tokenString = "invalid_refresh_token";

        _tokenRepositoryMock
            .Setup(repo => repo.GetRefreshTokenByTokenAsync(tokenString))
            .ReturnsAsync((RefreshToken)null!);

        // When ... Then
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _authenticationService.LogoutAsync(tokenString));

        _tokenRepositoryMock.Verify(repo => repo.GetRefreshTokenByTokenAsync(tokenString), Times.Once);
        _tokenRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<RefreshToken>()), Times.Never);
    }

    [Test]
    public void GivenUsedRefreshToken_WhenLogout_ThenUnauthorizedAccessExceptionIsThrown()
    {
        // Given
        var refreshTokenId = new RefreshTokenId(Guid.NewGuid());
        var userId = new UserId(Guid.NewGuid());
        const string jwtTokenId = "jwt_token_id";
        const string tokenString = "used_refresh_token";
        var token = RefreshToken.Create(
            refreshTokenId,
            userId,
            jwtTokenId,
            tokenString,
            false,
            DateTime.UtcNow.AddMinutes(10));

        _tokenRepositoryMock
            .Setup(repo => repo.GetRefreshTokenByTokenAsync(tokenString))
            .ReturnsAsync(token);

        // When ... Then
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _authenticationService.LogoutAsync(tokenString));

        _tokenRepositoryMock.Verify(repo => repo.GetRefreshTokenByTokenAsync(tokenString), Times.Once);
        _tokenRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<RefreshToken>()), Times.Never);
    }
}