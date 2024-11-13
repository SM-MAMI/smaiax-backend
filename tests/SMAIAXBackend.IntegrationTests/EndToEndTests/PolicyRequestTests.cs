using System.Net;
using System.Net.Http.Headers;
using System.Text;

using Newtonsoft.Json;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.IntegrationTests.EndToEndTests;

[TestFixture]
public class PolicyRequestTests : TestBase
{
    private const string BaseUrl = "/api/policyRequests";

    [Test]
    public async Task GivenPolicyRequestCreateDtoAndAccessToken_WhenCreatePolicyRequest_ThenMatchingPoliciesAreReturned()
    {
        // Given
        const int policyCountExpected = 1;
        var policyIdExpected = Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39");
        var policyRequestCreateDto = new PolicyRequestCreateDto(
            isAutomaticContractingEnabled: true,
            measurementResolution: MeasurementResolution.Hour,
            minHouseHoldSize: 1,
            maxHouseHoldSize: 5,
            locations: [],
            locationResolution: LocationResolution.Country,
            maxPrice: 500);

        var httpContent = new StringContent(JsonConvert.SerializeObject(policyRequestCreateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.PostAsync(BaseUrl, httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var policyDtos = JsonConvert.DeserializeObject<List<PolicyDto>>(responseContent);
        Assert.That(policyDtos, Is.Not.Null);
        Assert.That(policyDtos, Has.Count.EqualTo(policyCountExpected));

        var policyDto = policyDtos[0];
        Assert.That(policyDto, Is.Not.Null);
        Assert.That(policyDto.Id, Is.EqualTo(policyIdExpected));
    }
}