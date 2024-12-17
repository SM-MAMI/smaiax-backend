using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.IntegrationTests.EndToEndTests;

[TestFixture]
public class MeasurementTests : TestBase
{
    private const string BaseUrl = "/api/measurements";

    [Test]
    public async Task GivenQueryParametersAndAccessToken_WhenGetMeasurements_ThenMeasurementsAreReturned()
    {
        // Given
        var smartMeterId = Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd");
        var startAt = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ssZ");
        var endAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.GetAsync(BaseUrl +
                                                  $"?smartMeterId={Uri.EscapeDataString(smartMeterId.ToString())}&endAt={Uri.EscapeDataString(endAt)}&startAt={Uri.EscapeDataString(startAt)}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseContent = await response.Content.ReadFromJsonAsync<IList<MeasurementRawDto>>();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GivenQueryParametersAndAccessToken_WhenGetMeasurements_ThenMeasurementsAreNotInRange()
    {
        // Given
        var smartMeterId = Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd");
        var startAt = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-ddTHH:mm:ssZ");
        var endAt = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ssZ");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.GetAsync(BaseUrl +
                                                  $"?smartMeterId={Uri.EscapeDataString(smartMeterId.ToString())}&endAt={Uri.EscapeDataString(endAt)}&startAt={Uri.EscapeDataString(startAt)}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseContent = await response.Content.ReadFromJsonAsync<IList<MeasurementRawDto>>();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Is.Empty);
    }

    [Test]
    public async Task GivenQueryParametersAndAccessToken_WhenGetMeasurements_ThenBadRequestIsReturned()
    {
        // Given
        var smartMeterId = Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd");
        var endAt = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ssZ");
        var startAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.GetAsync(BaseUrl +
                                                  $"?smartMeterId={Uri.EscapeDataString(smartMeterId.ToString())}&endAt={Uri.EscapeDataString(endAt)}&startAt={Uri.EscapeDataString(startAt)}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var responseContent = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(responseContent?.Detail, Is.Not.Null);
        Assert.That(responseContent.Detail, Is.EqualTo("StartAt must be less than or equal to endAt."));
    }

    [Test]
    public async Task GivenQueryParametersAndAccessToken_WhenGetMeasurements_ThenSmartMeterNotFoundIsReturned()
    {
        // Given
        var smartMeterId = Guid.Parse("1ea76ec6-f9cf-417e-b3ad-cb672a98f53f");
        var startAt = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ssZ");
        var endAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        var expectedDetails = $"Smart meter with id '{smartMeterId} not found.";
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.GetAsync(BaseUrl +
                                                  $"?smartMeterId={Uri.EscapeDataString(smartMeterId.ToString())}&endAt={Uri.EscapeDataString(endAt)}&startAt={Uri.EscapeDataString(startAt)}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        var responseContent = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(responseContent?.Detail, Is.Not.Null);
        Assert.That(responseContent.Detail, Is.EqualTo(expectedDetails));
    }

    [Test]
    public async Task GivenQueryParametersAndNoAccessToken_WhenGetMeasurements_ThenUnauthorizedIsReturned()
    {
        // Given
        var smartMeterId = Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd");
        var startAt = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ssZ");
        var endAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.GetAsync(BaseUrl +
                                                  $"?smartMeterId={Uri.EscapeDataString(smartMeterId.ToString())}&endAt={Uri.EscapeDataString(endAt)}&startAt={Uri.EscapeDataString(startAt)}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}