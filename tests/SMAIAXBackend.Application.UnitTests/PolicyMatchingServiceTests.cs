using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class PolicyMatchingServiceTests
{
    private Mock<IPolicyRequestRepository> _policyRequestRepositoryMock;
    private Mock<ITenantRepository> _tenantRepositoryMock;
    private Mock<IPolicyRepository> _policyRepositoryMock;
    private Mock<ITenantContextService> _tenantContextServiceMock;
    private Mock<ILogger<PolicyMatchingService>> _loggerMock;
    private PolicyMatchingService _policyMatchingService;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _policyRequestRepositoryMock = new Mock<IPolicyRequestRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _policyRepositoryMock = new Mock<IPolicyRepository>();
        _tenantContextServiceMock = new Mock<ITenantContextService>();
        _loggerMock = new Mock<ILogger<PolicyMatchingService>>();
        _policyMatchingService = new PolicyMatchingService(
            _policyRequestRepositoryMock.Object,
            _tenantRepositoryMock.Object,
            _policyRepositoryMock.Object,
            _tenantContextServiceMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task GivenPolicyRequestId_WhenGetMatchingPolicies_ThenMatchingPoliciesAreReturned()
    {
        // Given
        const int policyCountExpected = 1;
        var policyRequestId = new PolicyRequestId(Guid.NewGuid());
        var policyFilter = new PolicyFilter(MeasurementResolution.Hour, 1, 5, [], LocationResolution.State, 100);
        var policyRequest = PolicyRequest.Create(policyRequestId, false, policyFilter);
        var tenants = new List<Tenant>
        {
            Tenant.Create(new TenantId(Guid.NewGuid()), "test", "test"
            ),
            Tenant.Create(
                new TenantId(Guid.NewGuid()),
                "test2",
                "test2"
            )
        };

        var policies = new List<Policy>
        {
            Policy.Create(
                new PolicyId(Guid.NewGuid()),
                "policy1",
                MeasurementResolution.Hour,
                LocationResolution.State,
                50,
                new SmartMeterId(Guid.NewGuid())
            ),
            Policy.Create(
                new PolicyId(Guid.NewGuid()),
                "policy2",
                MeasurementResolution.Hour,
                LocationResolution.State,
                150,
                new SmartMeterId(Guid.NewGuid())
            )
        };

        _policyRequestRepositoryMock.Setup(x => x.GetPolicyRequestByIdAsync(policyRequestId))
            .ReturnsAsync(policyRequest);
        _tenantContextServiceMock.Setup(service => service.GetCurrentTenantAsync()).ReturnsAsync(tenants[0]);
        _tenantRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tenants);
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesByTenantAsync(tenants[1])).ReturnsAsync(policies);

        // When
        var matchingPolicies = await _policyMatchingService.GetMatchingPoliciesAsync(policyRequestId.Id);

        // Then
        Assert.That(matchingPolicies, Is.Not.Empty);
        Assert.That(matchingPolicies, Has.Count.EqualTo(policyCountExpected));
        Assert.That(matchingPolicies[0].Id, Is.EqualTo(policies[0].Id.Id));
    }
}