using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class PolicyRequestCreateServiceTests
{
    private Mock<IPolicyRequestRepository> _policyRequestRepositoryMock;
    private Mock<ITenantRepository> _tenantRepositoryMock;
    private Mock<IPolicyRepository> _policyRepositoryMock;
    private Mock<ITenantContextService> _tenantContextServiceMock;
    private PolicyRequestCreateService _policyRequestCreateService;

    [SetUp]
    public void Setup()
    {
        _policyRequestRepositoryMock = new Mock<IPolicyRequestRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _policyRepositoryMock = new Mock<IPolicyRepository>();
        _tenantContextServiceMock = new Mock<ITenantContextService>();
        _policyRequestCreateService =
            new PolicyRequestCreateService(_policyRequestRepositoryMock.Object, _tenantRepositoryMock.Object,
                _policyRepositoryMock.Object, _tenantContextServiceMock.Object);
    }

    [Test]
    public async Task GivenPolicyRequestCreateDtoAndExistentUserId_WhenCreatePolicyRequest_ThenMatchingPoliciesAreReturned()
    {
        // Given
        var policyRequestIdExpected = new PolicyRequestId(Guid.NewGuid());
        var policyRequestCreateDto = new PolicyRequestCreateDto(
            isAutomaticContractingEnabled: true,
            measurementResolution: MeasurementResolution.Hour,
            minHouseHoldSize: 1,
            maxHouseHoldSize: 5,
            locations:
            [
                new LocationDto(
                    streetName: "Test Street",
                    city: "Test City",
                    state: "Test State",
                    country: "Test Country",
                    continent: Continent.Africa
                )
            ],
            locationResolution: LocationResolution.State,
            maxPrice: 100);
        var tenants = new List<Tenant>
        {
            Tenant.Create(
                id: new TenantId(Guid.NewGuid()),
                databaseUsername: "test",
                databasePassword: "test",
                databaseName: "test"
            ),
            Tenant.Create(
                id: new TenantId(Guid.NewGuid()),
                databaseUsername: "test2",
                databasePassword: "test2",
                databaseName: "test2"
            )
        };

        var policies = new List<Policy>
        {
            Policy.Create(
                id: new PolicyId(Guid.NewGuid()),
                measurementResolution: MeasurementResolution.Hour,
                locationResolution: LocationResolution.State,
                price: 50,
                smartMeterId: new SmartMeterId(Guid.NewGuid())
            ),
            Policy.Create(
                id: new PolicyId(Guid.NewGuid()),
                measurementResolution: MeasurementResolution.Hour,
                locationResolution: LocationResolution.State,
                price: 150,
                smartMeterId: new SmartMeterId(Guid.NewGuid())
            )
        };

        _policyRequestRepositoryMock.Setup(repo => repo.NextIdentity()).Returns(policyRequestIdExpected);
        _tenantContextServiceMock.Setup(service => service.GetCurrentTenantAsync()).ReturnsAsync(tenants[0]);
        _tenantRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tenants);
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesByTenantAsync(tenants[1])).ReturnsAsync(policies);

        // When
        var matchingPolicies = await _policyRequestCreateService.CreatePolicyRequestAsync(policyRequestCreateDto);

        // Then
        Assert.That(matchingPolicies, Is.Not.Empty);
    }
}