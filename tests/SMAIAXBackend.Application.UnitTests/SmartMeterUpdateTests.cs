using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterUpdateTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<ITenantRepository> _tenantRepositoryMock;
    private Mock<IUserValidationService> _userValidationServiceMock;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private Mock<ILogger<SmartMeterUpdateService>> _loggerMock;
    private SmartMeterUpdateService _smartMeterUpdateService;

    [SetUp]
    public void Setup()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _userValidationServiceMock = new Mock<IUserValidationService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _loggerMock = new Mock<ILogger<SmartMeterUpdateService>>();
        _smartMeterUpdateService = new SmartMeterUpdateService(_smartMeterRepositoryMock.Object,
            _tenantRepositoryMock.Object, _userValidationServiceMock.Object, _httpContextAccessorMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task GivenSmartMeterIdAndSmartMeterUpdateDtoAndUserId_WhenUpdateSmartMeter_ThenSmartMeterIdIsReturned()
    {
        // Given
        var smartMeterIdExpected = Guid.NewGuid();
        var smartMeterUpdateDto = new SmartMeterUpdateDto(smartMeterIdExpected, "Updated name");
        var tenant = Tenant.Create(new TenantId(Guid.NewGuid()), "test", "test", "test");
        var user = User.Create(new UserId(Guid.NewGuid()), new Name("Test", "Test"), "test", "test@example.com",
            tenant.Id);
        var smartMeter = SmartMeter.Create(new SmartMeterId(smartMeterIdExpected), "Name", user.Id);

        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext!.Items["UserId"]).Returns(user.Id.ToString());
        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(user.Id.ToString()))
            .ReturnsAsync(user);
        _tenantRepositoryMock.Setup(repo => repo.GetByIdAsync(tenant.Id)).ReturnsAsync(tenant);
        _smartMeterRepositoryMock.Setup(repo =>
                repo.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterIdExpected), tenant))
            .ReturnsAsync(smartMeter);

        // When
        Guid smartMeterIdActual =
            await _smartMeterUpdateService.UpdateSmartMeterAsync(smartMeterIdExpected, smartMeterUpdateDto);

        // Then
        Assert.That(smartMeterIdActual, Is.EqualTo(smartMeterIdExpected));
    }

    [Test]
    public void GivenNotMatchingSmartMeterIdsAndUserId_WhenUpdateSmartMeter_ThenSmartMeterIdMismatchExceptionIsThrown()
    {
        // Given
        var smartMeterIdExpected = Guid.NewGuid();
        var smartMeterUpdateDto = new SmartMeterUpdateDto(Guid.NewGuid(), "Updated name");
        var tenant = Tenant.Create(new TenantId(Guid.NewGuid()), "test", "test", "test");
        var user = User.Create(new UserId(Guid.NewGuid()), new Name("Test", "Test"), "test", "test@example.com",
            tenant.Id);

        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext!.Items["UserId"]).Returns(user.Id.ToString());
        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(user.Id.ToString()))
            .ReturnsAsync(user);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterIdMismatchException>(async () =>
            await _smartMeterUpdateService.UpdateSmartMeterAsync(smartMeterIdExpected, smartMeterUpdateDto));
    }

    [Test]
    public void
        GivenNonExistentSmartMeterIdAndSmartMeterUpdateDtoAndUserId_WhenUpdateSmartMeter_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var smartMeterIdExpected = Guid.NewGuid();
        var smartMeterUpdateDto = new SmartMeterUpdateDto(smartMeterIdExpected, "Updated name");
        var tenant = Tenant.Create(new TenantId(Guid.NewGuid()), "test", "test", "test");
        var user = User.Create(new UserId(Guid.NewGuid()), new Name("Test", "Test"), "test", "test@example.com",
            tenant.Id);

        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext!.Items["UserId"]).Returns(user.Id.ToString());
        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(user.Id.ToString()))
            .ReturnsAsync(user);
        _tenantRepositoryMock.Setup(repo => repo.GetByIdAsync(tenant.Id)).ReturnsAsync(tenant);
        _smartMeterRepositoryMock.Setup(repo =>
                repo.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterIdExpected), tenant))
            .ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(async () =>
            await _smartMeterUpdateService.UpdateSmartMeterAsync(smartMeterIdExpected, smartMeterUpdateDto));
    }
}