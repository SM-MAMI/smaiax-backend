using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class PolicyListServiceTests
{
    private Mock<IPolicyRepository> _policyRepositoryMock;
    private Mock<ILogger<PolicyListService>> _loggerMock;
    private PolicyListService _policyListService;
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _policyRepositoryMock = new Mock<IPolicyRepository>();
        _loggerMock = new Mock<ILogger<PolicyListService>>();
        _policyListService = new PolicyListService(_policyRepositoryMock.Object, _loggerMock.Object);
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
            Policy.Create(policyId2,  "policy2", MeasurementResolution.Hour, LocationResolution.None, 100, 
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
}