using Microsoft.AspNetCore.Http;
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
public class TenantContextServiceTests
{
    private Mock<ITenantRepository> _tenantRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private Mock<ILogger<TenantContextService>> _loggerMock;
    private TenantContextService _tenantContextService;

    [SetUp]
    public void SetUp()
    {
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _loggerMock = new Mock<ILogger<TenantContextService>>();

        _tenantContextService = new TenantContextService(
            _tenantRepositoryMock.Object,
            _userRepositoryMock.Object,
            _httpContextAccessorMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task GivenUserId_WhenGetTenant_ThenTenantIsReturned()
    {
        // Given
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var user = User.Create(new UserId(userId), new Name("John", "Doe"), "johndoe", "john.doe@example.com",
            new TenantId(tenantId));
        var tenant = Tenant.Create(new TenantId(tenantId), "test", "test", "test");

        _httpContextAccessorMock.Setup(httpContextAccessor => httpContextAccessor.HttpContext!.Items["UserId"])
            .Returns(userId.ToString());
        _userRepositoryMock.Setup(userRepository => userRepository.GetUserByIdAsync(new UserId(userId)))
            .ReturnsAsync(user);
        _tenantRepositoryMock.Setup(tenantRepository => tenantRepository.GetByIdAsync(new TenantId(tenantId)))
            .ReturnsAsync(tenant);

        // When
        var tenantActual = await _tenantContextService.GetCurrentTenantAsync();

        // Then
        Assert.That(tenantActual, Is.EqualTo(tenant));
    }

    [Test]
    public void GivenNoUserId_WhenGetTenant_ThenInvalidTokenExceptionIsThrown()
    {
        // Given
        _httpContextAccessorMock.Setup(httpContextAccessor => httpContextAccessor.HttpContext!.Items["UserId"])
            .Returns((string?)null);

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () => await _tenantContextService.GetCurrentTenantAsync());
    }

    [Test]
    public void GivenInvalidUserId_WhenGetTenant_ThenInvalidTokenExceptionIsThrown()
    {
        // Given
        _httpContextAccessorMock.Setup(httpContextAccessor => httpContextAccessor.HttpContext!.Items["UserId"])
            .Returns("42");

        // When ... Then
        Assert.ThrowsAsync<InvalidTokenException>(async () => await _tenantContextService.GetCurrentTenantAsync());
    }

    [Test]
    public void GivenNonExistentUser_WhenGetTenant_ThenUserNotFoundExceptionIsThrown()
    {
        // Given
        var userId = Guid.NewGuid();

        _httpContextAccessorMock.Setup(httpContextAccessor => httpContextAccessor.HttpContext!.Items["UserId"])
            .Returns(userId.ToString());
        _userRepositoryMock.Setup(userRepository => userRepository.GetUserByIdAsync(new UserId(userId)))
            .ReturnsAsync((User)null!);

        // When ... Then
        Assert.ThrowsAsync<UserNotFoundException>(async () => await _tenantContextService.GetCurrentTenantAsync());
    }

    [Test]
    public void GivenNonExistentTenant_WhenGetTenant_ThenTenantNotFoundExceptionIsThrown()
    {
        // Given
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var user = User.Create(new UserId(userId), new Name("John", "Doe"), "johndoe", "john.doe@example.com",
            new TenantId(tenantId));

        _httpContextAccessorMock.Setup(httpContextAccessor => httpContextAccessor.HttpContext!.Items["UserId"]).Returns(userId.ToString());
        _userRepositoryMock.Setup(userRepository => userRepository.GetUserByIdAsync(new UserId(userId)))
            .ReturnsAsync(user);
        _tenantRepositoryMock.Setup(tenantRepository => tenantRepository.GetByIdAsync(new TenantId(tenantId)))
            .ReturnsAsync(
                (Tenant)null!);

        // When ... Then
        Assert.ThrowsAsync<TenantNotFoundException>(async () => await _tenantContextService.GetCurrentTenantAsync());
    }
}