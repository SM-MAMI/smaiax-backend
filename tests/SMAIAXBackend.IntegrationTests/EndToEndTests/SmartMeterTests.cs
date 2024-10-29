using System.Net;
using System.Net.Http.Headers;
using System.Text;

using Newtonsoft.Json;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.IntegrationTests.EndToEndTests;

[TestFixture]
public class SmartMeterTests : TestBase
{
    private const string BaseUrl = "/api/smartMeters";

    [Test]
    public async Task GivenSmartMeterCreateDtoAndAccessToken_WhenCreateSmartMeter_ThenSmartMeterIsCreated()
    {
        // Given
        var smartMeterCreateDto = new SmartMeterCreateDto("Test Smart Meter");
        var httpContent = new StringContent(JsonConvert.SerializeObject(smartMeterCreateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.PostAsync(BaseUrl, httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var responseGuidString = responseContent.Trim('"');
        var smartMeterActual =
            _applicationDbContext.SmartMeters.FirstOrDefault(x =>
                x.Id.Equals(new SmartMeterId(Guid.Parse(responseGuidString))));

        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterCreateDto.Name));
    }

    [Test]
    public async Task GivenSmartMeterCreateDtoAndNoAccessToken_WhenCreateSmartMeter_ThenUnauthorizedIsReturned()
    {
        // Given
        var smartMeterCreateDto = new SmartMeterCreateDto("Test Smart Meter");
        var httpContent = new StringContent(JsonConvert.SerializeObject(smartMeterCreateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.PostAsync(BaseUrl, httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenAccessToken_WhenGetSmartMeters_ThenExpectedSmartMetersAreReturned()
    {
        // Given
        var smartMetersExpected = new List<SmartMeterOverviewDto>()
        {
            new(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd"), "Smart Meter 1", 0, 0),
            new(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39"), "Smart Meter 2", 0, 0)
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.GetAsync(BaseUrl);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var smartMetersActual = JsonConvert.DeserializeObject<List<SmartMeterOverviewDto>>(responseContent);
        Assert.That(smartMetersActual, Is.Not.Null);
        Assert.That(smartMetersActual, Has.Count.EqualTo(smartMetersExpected.Count));

        for (int i = 0; i < smartMetersActual.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(smartMetersActual[i].Id, Is.EqualTo(smartMetersExpected[i].Id));
                Assert.That(smartMetersActual[i].Name, Is.EqualTo(smartMetersExpected[i].Name));
                Assert.That(smartMetersActual[i].MetadataCount, Is.EqualTo(smartMetersExpected[i].MetadataCount));
                Assert.That(smartMetersActual[i].PolicyCount, Is.EqualTo(smartMetersExpected[i].PolicyCount));
            });
        }
    }

    [Test]
    public async Task GivenNoAccessToken_WhenGetSmartMeters_ThenUnauthorizedIsReturned()
    {
        // Given
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.GetAsync(BaseUrl);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenSmartMeterIdAndAccessToken_WhenGetSmartMeterById_ThenExpectedSmartMetersAreReturned()
    {
        // Given
        var smartMeterExpected =
            new SmartMeterOverviewDto(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd"), "Smart Meter 1", 0, 0);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.GetAsync($"{BaseUrl}/{smartMeterExpected.Id}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var smartMeterActual = JsonConvert.DeserializeObject<SmartMeterOverviewDto>(responseContent);
        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(smartMeterActual.Id, Is.EqualTo(smartMeterExpected.Id));
            Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterExpected.Name));
            Assert.That(smartMeterActual.MetadataCount, Is.EqualTo(smartMeterExpected.MetadataCount));
            Assert.That(smartMeterActual.PolicyCount, Is.EqualTo(smartMeterExpected.PolicyCount));
        });
    }

    [Test]
    public async Task GivenSmartMeterIdAndNoAccessToken_WhenGetSmartMeterById_ThenUnauthorizedIsReturned()
    {
        // Given
        var smartMeterId = Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.GetAsync($"{BaseUrl}/{smartMeterId}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}