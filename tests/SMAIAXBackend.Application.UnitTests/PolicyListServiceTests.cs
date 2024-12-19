using Moq;

using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class PolicyListServiceTests
{
    private Mock<IPolicyRepository> _policyRepositoryMock;
    private Mock<ITenantRepository> _tenantRepositoryMock;
    private Mock<ITenantContextService> _tenantContextServiceMock;
    private PolicyListService _policyListService;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _policyRepositoryMock = new Mock<IPolicyRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _tenantContextServiceMock = new Mock<ITenantContextService>();
        _policyListService = new PolicyListService(_policyRepositoryMock.Object, _tenantRepositoryMock.Object,
            _tenantContextServiceMock.Object);
    }

    [Test]
    public async Task GivenExistingPolicies_WhenGetPolicies_ThenReturnPolicies()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var policyId1 = new PolicyId(Guid.NewGuid());
        var policyId2 = new PolicyId(Guid.NewGuid());

        var policiesExpected = new List<Policy>
        {
            Policy.Create(policyId1, "policy1", MeasurementResolution.Hour, LocationResolution.None, 100,
                smartMeterId),
            Policy.Create(policyId2, "policy2", MeasurementResolution.Hour, LocationResolution.None, 100,
                smartMeterId)
        };

        _policyRepositoryMock.Setup(repo => repo.GetPoliciesAsync()).ReturnsAsync(policiesExpected);

        // When
        var policiesActual = await _policyListService.GetPoliciesAsync();

        // Then
        Assert.That(policiesActual, Is.Not.Null);
        Assert.That(policiesActual, Has.Count.EqualTo(policiesExpected.Count));
    }

    [Test]
    public async Task GivenNoPolicies_WhenGetPolicies_ThenReturnEmptyList()
    {
        // Given
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesAsync()).ReturnsAsync(new List<Policy>());

        // When
        var policiesActual = await _policyListService.GetPoliciesAsync();

        // Then
        Assert.That(policiesActual, Is.Not.Null);
        Assert.That(policiesActual, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GivenExistingPolicies_WhenGetPoliciesBySmartMeterId_ThenReturnPolicies()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var policyId1 = new PolicyId(Guid.NewGuid());
        var policyId2 = new PolicyId(Guid.NewGuid());

        var policiesExpected = new List<Policy>
        {
            Policy.Create(policyId1, "policy1", MeasurementResolution.Hour, LocationResolution.None, 100,
                smartMeterId),
            Policy.Create(policyId2, "policy2", MeasurementResolution.Hour, LocationResolution.None, 100,
                smartMeterId)
        };

        _policyRepositoryMock.Setup(repo => repo.GetPoliciesBySmartMeterIdAsync(smartMeterId))
            .ReturnsAsync(policiesExpected);

        // When
        var policiesActual = await _policyListService.GetPoliciesBySmartMeterIdAsync(smartMeterId);

        // Then
        Assert.That(policiesActual, Is.Not.Null);
        Assert.That(policiesActual, Has.Count.EqualTo(policiesExpected.Count));
    }

    [Test]
    public async Task GivenNoPolicies_WhenGetPoliciesBySmartMeterId_ThenReturnEmptyList()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());

        _policyRepositoryMock.Setup(repo => repo.GetPoliciesBySmartMeterIdAsync(smartMeterId))
            .ReturnsAsync(new List<Policy>());

        // When
        var policiesActual = await _policyListService.GetPoliciesBySmartMeterIdAsync(smartMeterId);

        // Then
        Assert.That(policiesActual, Is.Not.Null);
        Assert.That(policiesActual, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GivenExistingPolicies_WhenGetFilteredPolicies_ThenReturnFilteredPolicies()
    {
        // Given
        const int maxPrice = 100;
        const MeasurementResolution measurementResolution = MeasurementResolution.Hour;
        var currentTenant = Tenant.Create(new TenantId(Guid.NewGuid()), "tenant1", "tenant1");
        List<Tenant> tenants =
        [
            Tenant.Create(new TenantId(Guid.NewGuid()), "tenant2", "tenant2"),
            Tenant.Create(new TenantId(Guid.NewGuid()), "tenant3", "tenant3")
        ];

        var policyId1 = new PolicyId(Guid.NewGuid());
        var policyId2 = new PolicyId(Guid.NewGuid());

        var policiesExpected = new List<Policy>
        {
            Policy.Create(policyId1, "policy1", MeasurementResolution.Hour, LocationResolution.None, 100,
                new SmartMeterId(Guid.NewGuid())),
            Policy.Create(policyId2, "policy2", MeasurementResolution.Hour, LocationResolution.None, 100,
                new SmartMeterId(Guid.NewGuid()))
        };

        _tenantContextServiceMock.Setup(service => service.GetCurrentTenantAsync()).ReturnsAsync(currentTenant);
        _tenantRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tenants);
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesByTenantAsync(tenants[0])).ReturnsAsync(policiesExpected);
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesByTenantAsync(tenants[1])).ReturnsAsync([]);

        // When
        var policiesActual = await _policyListService.GetFilteredPoliciesAsync(maxPrice, measurementResolution);

        // Then
        Assert.That(policiesActual, Is.Not.Null);
        Assert.That(policiesActual, Has.Count.EqualTo(policiesExpected.Count));
    }

    [Test]
    public async Task GivenNoPolicies_WhenGetFilteredPolicies_ThenReturnEmptyList()
    {
        // Given
        const int maxPrice = 100;
        const MeasurementResolution measurementResolution = MeasurementResolution.Hour;
        var currentTenant = Tenant.Create(new TenantId(Guid.NewGuid()), "tenant1", "tenant1");
        List<Tenant> tenants =
        [
            Tenant.Create(new TenantId(Guid.NewGuid()), "tenant2", "tenant2"),
            Tenant.Create(new TenantId(Guid.NewGuid()), "tenant3", "tenant3")
        ];

        _tenantContextServiceMock.Setup(service => service.GetCurrentTenantAsync()).ReturnsAsync(currentTenant);
        _tenantRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tenants);
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesByTenantAsync(It.IsAny<Tenant>())).ReturnsAsync([]);

        // When
        var policiesActual = await _policyListService.GetFilteredPoliciesAsync(maxPrice, measurementResolution);

        // Then
        Assert.That(policiesActual, Is.Not.Null);
        Assert.That(policiesActual, Has.Count.EqualTo(0));
    }
}