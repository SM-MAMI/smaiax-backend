using System.Net;
using System.Net.Http.Headers;
using System.Text;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.IntegrationTests.EndToEndTests;

[TestFixture]
public class PolicyTests : TestBase
{
    private const string BaseUrl = "/api/policies";

    [Test]
    public async Task GivenPolicyCreateDtoAndAccessToken_WhenCreatePolicy_ThenPolicyIsCreated()
    {
        // Given
        var smartMeterId = Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39");
        var policyCreateDto = new PolicyCreateDto("policy1", MeasurementResolution.Minute, LocationResolution.City, 100, smartMeterId);

        var httpContent = new StringContent(JsonConvert.SerializeObject(policyCreateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.PostAsync(BaseUrl, httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var returnedId = Guid.Parse(responseContent.Trim('"'));
        var policyActual = await _tenant1DbContext.Policies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(new PolicyId(returnedId)));

        Assert.That(policyActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(policyActual.MeasurementResolution, Is.EqualTo(policyCreateDto.MeasurementResolution));
            Assert.That(policyActual.LocationResolution, Is.EqualTo(policyCreateDto.LocationResolution));
            Assert.That(policyActual.Price, Is.EqualTo(policyCreateDto.Price));
            Assert.That(policyActual.State, Is.EqualTo(PolicyState.Active));
        });
    }

    [Test]
    public async Task GivenPolicyCreateDtoAndNoAccessToken_WhenCreatePolicy_ThenUnauthorizedIsReturned()
    {
        // Given
        var smartMeterId = Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39");
        var policyCreateDto = new PolicyCreateDto("policy1", MeasurementResolution.Minute, LocationResolution.City, 100, smartMeterId);

        var httpContent = new StringContent(JsonConvert.SerializeObject(policyCreateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.PostAsync(BaseUrl, httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
    
    [Test]
    public async Task GivenPoliciesExist_WhenGetPolicies_ThenReturnPolicies()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var policy1 = Policy.Create(new PolicyId(Guid.NewGuid()), "policy1", MeasurementResolution.Hour, LocationResolution.City, 100, smartMeterId);
        var policy2 = Policy.Create(new PolicyId(Guid.NewGuid()), "policy2", MeasurementResolution.Hour, LocationResolution.City, 100, smartMeterId);
        await _tenant1DbContext.Policies.AddRangeAsync(policy1, policy2);
        await _tenant1DbContext.SaveChangesAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.GetAsync(BaseUrl);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var policies = JsonConvert.DeserializeObject<List<PolicyDto>>(responseContent);
        Assert.That(policies, Is.Not.Null);
        Assert.That(policies, Has.Count.EqualTo(2));
    }
    
    [Test]
    public async Task GivenNoPoliciesExist_WhenGetPolicies_ThenReturnEmptyList()
    {
        // Given
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.GetAsync(BaseUrl);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var policies = JsonConvert.DeserializeObject<List<PolicyDto>>(responseContent);
        Assert.That(policies, Is.Not.Null);
        Assert.That(policies, Has.Count.EqualTo(0));
    }
    
    [Test]
    public async Task GivenPoliciesExist_WhenGetPoliciesBySmartMeterId_ThenReturnPolicies()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var policy1 = Policy.Create(new PolicyId(Guid.NewGuid()), "policy1", MeasurementResolution.Hour, LocationResolution.City, 100, smartMeterId);
        var policy2 = Policy.Create(new PolicyId(Guid.NewGuid()), "policy2", MeasurementResolution.Hour, LocationResolution.City, 100, smartMeterId);
        await _tenant1DbContext.Policies.AddRangeAsync(policy1, policy2);
        await _tenant1DbContext.SaveChangesAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.GetAsync($"{BaseUrl}?smartMeterId={smartMeterId}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var policies = JsonConvert.DeserializeObject<List<PolicyDto>>(responseContent);
        Assert.That(policies, Is.Not.Null);
        Assert.That(policies, Has.Count.EqualTo(2));
    }
    
    [Test]
    public async Task GivenNoPoliciesExist_WhenGetPoliciesBySmartMeterId_ThenReturnEmptyList()
    {
        // Given
        var smartMeterId = Guid.NewGuid();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.GetAsync($"{BaseUrl}?smartMeterId={smartMeterId}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var policies = JsonConvert.DeserializeObject<List<PolicyDto>>(responseContent);
        Assert.That(policies, Is.Not.Null);
        Assert.That(policies, Has.Count.EqualTo(0));
    }
}