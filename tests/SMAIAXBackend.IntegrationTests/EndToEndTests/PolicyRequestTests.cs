using System.Net;
using System.Net.Http.Headers;
using System.Text;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.IntegrationTests.EndToEndTests;

[TestFixture]
public class PolicyRequestTests : TestBase
{
    private const string BaseUrl = "/api/policyRequests";

    [Test]
    public async Task GivenPolicyRequestCreateDtoAndAccessToken_WhenCreatePolicyRequest_ThenPolicyRequestIsCreated()
    {
        // Given
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

        var httpContent = new StringContent(JsonConvert.SerializeObject(policyRequestCreateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.PostAsync(BaseUrl, httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var returnedId = Guid.Parse(responseContent.Trim('"'));
        var policyRequestActual = await _applicationDbContext.PolicyRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(new PolicyRequestId(returnedId)));

        Assert.That(policyRequestActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(policyRequestActual.IsAutomaticContractingEnabled, Is.EqualTo(policyRequestCreateDto.IsAutomaticContractingEnabled));
            Assert.That(policyRequestActual.PolicyFilter.MeasurementResolution, Is.EqualTo(policyRequestCreateDto.MeasurementResolution));
            Assert.That(policyRequestActual.PolicyFilter.MinHouseHoldSize, Is.EqualTo(policyRequestCreateDto.MinHouseHoldSize));
            Assert.That(policyRequestActual.PolicyFilter.MaxHouseHoldSize, Is.EqualTo(policyRequestCreateDto.MaxHouseHoldSize));
            Assert.That(policyRequestActual.PolicyFilter.Locations.Count, Is.EqualTo(policyRequestCreateDto.Locations.Count));
        });
    }
}