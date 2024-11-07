using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterListServiceTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<ITenantContextService> _tenantContextServiceMock;
    private Mock<ILogger<SmartMeterListService>> _loggerMock;
    private SmartMeterListService _smartMeterListService;

    [SetUp]
    public void Setup()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _tenantContextServiceMock = new Mock<ITenantContextService>();
        _loggerMock = new Mock<ILogger<SmartMeterListService>>();
        _smartMeterListService =
            new SmartMeterListService(_smartMeterRepositoryMock.Object, _tenantContextServiceMock.Object,
                _loggerMock.Object);
    }

    [Test]
    public async Task GivenUserId_WhenGetSmartMetersByUserId_ThenExpectedSmartMetersAreReturned()
    {
        // Given
        var tenant = Tenant.Create(new TenantId(Guid.NewGuid()), "test", "test", "test");
        var smartMetersExpected = new List<SmartMeter>
        {
            SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 1"),
            SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 2")
        };

        _tenantContextServiceMock.Setup(service => service.GetCurrentTenantAsync()).ReturnsAsync(tenant);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMetersAsync(tenant))
            .ReturnsAsync(smartMetersExpected);

        // When
        var smartMetersActual = await _smartMeterListService.GetSmartMetersAsync();

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
                Assert.That(smartMetersActual[i].PolicyCount, Is.EqualTo(smartMetersExpected[i].Policies.Count));
            });
        }
    }

    [Test]
    public async Task
        GivenValidSmartMeterIdAndUserId_WhenGetSmartMeterByIdAndUserIdAsync_ThenExpectedSmartMeterIsReturned()
    {
        // Given
        var smartMeterId = Guid.NewGuid();
        var tenant = Tenant.Create(new TenantId(Guid.NewGuid()), "test", "test", "test");
        var smartMeterExpected = SmartMeter.Create(new SmartMeterId(smartMeterId), "Smart Meter 1");

        _tenantContextServiceMock.Setup(service => service.GetCurrentTenantAsync()).ReturnsAsync(tenant);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAsync(smartMeterExpected.Id, tenant))
            .ReturnsAsync(smartMeterExpected);

        // When
        var smartMeterActual =
            await _smartMeterListService.GetSmartMeterByIdAsync(smartMeterId);

        // Then
        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(smartMeterActual.Id, Is.EqualTo(smartMeterExpected.Id.Id));
            Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterExpected.Name));
            Assert.That(smartMeterActual.MetadataCount, Is.EqualTo(smartMeterExpected.Metadata.Count));
            Assert.That(smartMeterActual.PolicyCount, Is.EqualTo(smartMeterExpected.Policies.Count));
        });
    }

    [Test]
    public void
        GivenNonExistentSmartMeterIdOrUserId_WhenGetSmartMeterByIdAndUserIdAsync_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var tenant = Tenant.Create(new TenantId(Guid.NewGuid()), "test", "test", "test");

        _tenantContextServiceMock.Setup(service => service.GetCurrentTenantAsync()).ReturnsAsync(tenant);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAsync(smartMeterId, tenant))
            .ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(async () =>
            await _smartMeterListService.GetSmartMeterByIdAsync(smartMeterId.Id)
        );
    }
}