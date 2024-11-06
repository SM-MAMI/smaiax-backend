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
public class PolicyTests : TestBase
{
    private const string BaseUrl = "/api/policies";

    [Test]
    public async Task GivenPolicyCreateDtoAndAccessToken_WhenCreatePolicy_ThenPolicyIsCreated()
    {
        // Given
        var locationDto = new LocationDto("Some street name", "Some city", "Some state", "Some country", Continent.Europe);
        var policyCreateDto = new PolicyCreateDto(MeasurementResolution.Minute, 4, locationDto, LocationResolution.City, 100);

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
        var policyActual = await _applicationDbContext.Policies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(new PolicyId(returnedId)));

        Assert.That(policyActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(policyActual.MeasurementResolution, Is.EqualTo(policyCreateDto.MeasurementResolution));
            Assert.That(policyActual.HouseholdSize, Is.EqualTo(policyCreateDto.HouseholdSize));
            Assert.That(policyActual.Location.StreetName, Is.EqualTo(policyCreateDto.Location.StreetName));
            Assert.That(policyActual.Location.City, Is.EqualTo(policyCreateDto.Location.City));
            Assert.That(policyActual.Location.State, Is.EqualTo(policyCreateDto.Location.State));
            Assert.That(policyActual.Location.Country, Is.EqualTo(policyCreateDto.Location.Country));
            Assert.That(policyActual.Location.Continent, Is.EqualTo(policyCreateDto.Location.Continent));
            Assert.That(policyActual.LocationResolution, Is.EqualTo(policyCreateDto.LocationResolution));
            Assert.That(policyActual.Price, Is.EqualTo(policyCreateDto.Price));
            Assert.That(policyActual.State, Is.EqualTo(PolicyState.Active));
        });
    }

    [Test]
    public async Task GivenPolicyCreateDtoAndNoAccessToken_WhenCreatePolicy_ThenUnauthorizedIsReturned()
    {
        // Given
        var locationDto = new LocationDto("Some street name", "Some city", "Some state", "Some country", Continent.Europe);
        var policyCreateDto = new PolicyCreateDto(MeasurementResolution.Minute, 4, locationDto, LocationResolution.City, 100);

        var httpContent = new StringContent(JsonConvert.SerializeObject(policyCreateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.PostAsync(BaseUrl, httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}