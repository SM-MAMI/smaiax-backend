using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterCreateServiceTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<ITenantRepository> _tenantRepositoryMock;
    private Mock<IUserValidationService> _userValidationServiceMock;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private Mock<ILogger<SmartMeterCreateService>> _loggerMock;
    private SmartMeterCreateService _smartMeterCreateService;

    [SetUp]
    public void Setup()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _userValidationServiceMock = new Mock<IUserValidationService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _loggerMock = new Mock<ILogger<SmartMeterCreateService>>();
        _smartMeterCreateService = new SmartMeterCreateService(_smartMeterRepositoryMock.Object,
            _tenantRepositoryMock.Object, _userValidationServiceMock.Object, _httpContextAccessorMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task GivenSmartMeterCreateDtoAndExistentUserId_WhenAddSmartMeter_ThenSmartMeterIdIsReturned()
    {
        // Given
        var smartMeterIdExpected = new SmartMeterId(Guid.NewGuid());
        var smartMeterCreateDto = new SmartMeterCreateDto("Test Smart Meter");
        var tenant = Tenant.Create(new TenantId(Guid.NewGuid()), "test", "test", "test");
        var user = User.Create(new UserId(Guid.NewGuid()), new Name("Test", "Test"), "test", "test@example.com",
            tenant.Id);

        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext!.Items["UserId"]).Returns(user.Id.ToString());
        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(user.Id.ToString()))
            .ReturnsAsync(user);
        _tenantRepositoryMock.Setup(repo => repo.GetByIdAsync(tenant.Id)).ReturnsAsync(tenant);
        _smartMeterRepositoryMock.Setup(repo => repo.NextIdentity()).Returns(smartMeterIdExpected);

        // When
        var smartMeterIdActual =
            await _smartMeterCreateService.AddSmartMeterAsync(smartMeterCreateDto);

        // Then
        Assert.That(smartMeterIdActual, Is.EqualTo(smartMeterIdExpected.Id));
    }
}