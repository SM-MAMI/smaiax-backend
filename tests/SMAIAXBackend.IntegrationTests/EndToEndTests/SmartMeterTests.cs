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
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

        // When
        var response = await HttpClient.PostAsync(BaseUrl, httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var responseGuidString = responseContent.Trim('"');
        var smartMeterActual =
            ApplicationDbContext.SmartMeters.FirstOrDefault(x =>
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
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await HttpClient.PostAsync(BaseUrl, httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}