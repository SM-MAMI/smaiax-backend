using System.Net;
using System.Text;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.IntegrationTests.EndToEndTests;

[TestFixture]
public class AuthenticationTests : TestBase
{
    private const string BaseUrl = "/api/authentication";

    [Test]
    public async Task GivenUserInformation_WhenRegister_ThenDomainUserAndIdentityUserAreCreated()
    {
        // Given
        var registerDto = new RegisterDto("user@example.com", "P@ssw0rd", new NameDto("John", "Doe"));

        var httpContent = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8,
            "application/json");

        // When
        var response = await _httpClient.PostAsync($"{BaseUrl}/register", httpContent);
        

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        
        var id = JsonConvert.DeserializeObject<Guid>(responseContent);
        var identityUser = await _applicationDbContext.Users
            .SingleOrDefaultAsync(u => u.Id == id.ToString());
        var domainUser = await _applicationDbContext.DomainUsers
            .SingleOrDefaultAsync(u => u.Id == new UserId(id));

        Assert.Multiple(() =>
        {
            Assert.That(identityUser, Is.Not.Null);
            Assert.That(domainUser, Is.Not.Null);
        });
        Assert.That(identityUser.Email, Is.EqualTo(registerDto.Email));
        Assert.Multiple(() =>
        {
            Assert.That(domainUser.Email, Is.EqualTo(registerDto.Email));
            Assert.That(domainUser.Name.FirstName, Is.EqualTo(registerDto.Name.FirstName));
            Assert.That(domainUser.Name.LastName, Is.EqualTo(registerDto.Name.LastName));
        });
    }

    [Test]
    public async Task GivenInvalidUserInformation_WhenRegister_ThenErrorResponseIsReturned()
    {
        // Given
        var registerDto = new RegisterDto("user@example.com", "Passw0rd", new NameDto("John", "Doe"));

        var httpContent = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8,
            "application/json");

        // When
        var response = await _httpClient.PostAsync($"{BaseUrl}/register", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Does.Contain("Registration Error"));
        Assert.That(responseContent,
            Does.Contain(
                "Registration failed with the following errors: Passwords must have at least one non alphanumeric character."));
    }

    [Test]
    public async Task GivenValidUsernameAndPassword_WhenLogin_ThenAccessTokenAndRefreshTokenAreReturned()
    {
        // Given
        var loginDto = new LoginDto("john.doe@example.com", "P@ssw0rd");
        var httpContent = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8,
            "application/json");

        // When
        var response = await _httpClient.PostAsync($"{BaseUrl}/login", httpContent);
       

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        
        var tokenDto = JsonConvert.DeserializeObject<TokenDto>(responseContent);
        Assert.That(tokenDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(tokenDto.AccessToken, Is.Not.Null);
            Assert.That(tokenDto.RefreshToken, Is.Not.Null);
        });
    }

    [Test]
    public async Task GivenInvalidUsernameAndValidPassword_WhenLogin_ThenErrorResponseIsReturned()
    {
        // Given
        var loginDto = new LoginDto("john.invalid@example.com", "P@ssw0rd");
        var httpContent = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8,
            "application/json");

        // When
        var response = await _httpClient.PostAsync($"{BaseUrl}/login", httpContent);
        
        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Does.Contain("Unauthorized"));
        Assert.That(responseContent, Does.Contain("Username or password is wrong"));
    }

    [Test]
    public async Task GivenValidUsernameAndInvalidPassword_WhenLogin_ThenErrorResponseIsReturned()
    {
        // Given
        var loginDto = new LoginDto("john.doe@example.com", "InvalidPassword");
        var httpContent = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8,
            "application/json");

        // When
        var response = await _httpClient.PostAsync($"{BaseUrl}/login", httpContent);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(responseContent, Is.Not.Null);
        });
        Assert.That(responseContent, Does.Contain("Unauthorized"));
        Assert.That(responseContent, Does.Contain("Username or password is wrong"));
    }

    [Test]
    public async Task
        GivenValidAccessTokenAndValidRefreshToken_WhenRefreshTokens_ThenAccessTokenAndRefreshTokensAreReturned()
    {
        // Given
        const string accessToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzYzA3MDY1YS1iOTY0LTQ0YTktOWNkZi1mYmQ0OWQ3NTVlYTciLCJ1bmlxdWVfbmFtZSI6ImpvaG4uZG9lQGV4YW1wbGUuY29tIiwianRpIjoiMTlmNzdiMmUtZTQ4NS00MDMxLTg1MDYtNjJmNmQzYjY5ZTRkIiwiaWF0IjoxNzI4Mzg5MzkxLCJleHAiOjE3MjgzOTI5OTEsImlzcyI6IlNNQUlBWCIsImF1ZCI6IlNvbWVBdWRpZW5jZSJ9.chHm391Tbcwo-Adq3QPPQses9NJuyUzM0vMjQUR6FmA";
        const string refreshToken = "4dffb63c-581d-4588-8b4b-4b075f17d015-abcb30f4-5f32-4fbb-80c4-99cea98273ca";
        var tokenDto = new TokenDto(accessToken, refreshToken);
        var httpContent = new StringContent(JsonConvert.SerializeObject(tokenDto), Encoding.UTF8,
            "application/json");

        // When
        var response = await _httpClient.PostAsync($"{BaseUrl}/refresh", httpContent);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(responseContent, Is.Not.Null);
        var responseTokenDto = JsonConvert.DeserializeObject<TokenDto>(responseContent);
        Assert.That(responseTokenDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseTokenDto.AccessToken, Is.Not.Null);
            Assert.That(responseTokenDto.RefreshToken, Is.Not.Null);
        });
    }

    [Test, Sequential]
    public async Task GivenValidAccessTokenAndInvalidRefreshToken_WhenRefreshTokens_ThenUnauthorizedResponseIsReturned(
        [Values("266cbbdb-edcd-48a6-aa63-f837b05a2551-3b01aaa3-304a-434b-bc7d-fd9a6305550",
            "01318f82-8307-480d-bbb6-f3be92ba7480-b903a69c-7d76-4ce1-8dce-d325712bf240")]
        string refreshToken)
    {
        // Given
        const string accessToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzYzA3MDY1YS1iOTY0LTQ0YTktOWNkZi1mYmQ0OWQ3NTVlYTciLCJ1bmlxdWVfbmFtZSI6ImpvaG4uZG9lQGV4YW1wbGUuY29tIiwianRpIjoiMTlmNzdiMmUtZTQ4NS00MDMxLTg1MDYtNjJmNmQzYjY5ZTRkIiwiaWF0IjoxNzI4Mzg5MzkxLCJleHAiOjE3MjgzOTI5OTEsImlzcyI6IlNNQUlBWCIsImF1ZCI6IlNvbWVBdWRpZW5jZSJ9.chHm391Tbcwo-Adq3QPPQses9NJuyUzM0vMjQUR6FmA";
        var tokenDto = new TokenDto(accessToken, refreshToken);
        var httpContent = new StringContent(JsonConvert.SerializeObject(tokenDto), Encoding.UTF8,
            "application/json");

        // When
        var response = await _httpClient.PostAsync($"{BaseUrl}/refresh", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenInvalidAccessTokenAndValidRefreshToken_WhenRefreshTokens_ThenUnauthorizedResponseIsReturned()
    {
        // Given
        const string accessToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxZTA5Y2EyOS0yOTEwLTRmNTQtODAwMi0yZDllMDYzMDkwYzYiLCJ1bmlxdWVfbmFtZSI6ImpvaG4uZG9lQGV4YW1wbGUuY29tIiwianRpIjoiODcwMzllMmYtODY3ZS00MDlkLWJiNjAtZThkYWJjODRmNTJkIiwiaWF0IjoxNzI4Mzg5MzkxLCJleHAiOjE3MjgzOTI5OTEsImlzcyI6IlNNQUlBWCIsImF1ZCI6IlNvbWVBdWRpZW5jZSJ9.o56mNYTTPlcO2YcEhRhiSsNyjz6l-yimImHUmBMNTEc";
        const string refreshToken = "4dffb63c-581d-4588-8b4b-4b075f17d015-abcb30f4-5f32-4fbb-80c4-99cea98273ca";
        var tokenDto = new TokenDto(accessToken, refreshToken);
        var httpContent = new StringContent(JsonConvert.SerializeObject(tokenDto), Encoding.UTF8,
            "application/json");

        // When
        var response = await _httpClient.PostAsync($"{BaseUrl}/refresh", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task
        GivenInvalidAccessTokenAndInvalidRefreshTokenWithNonExistentUserId_WhenRefreshTokens_ThenUnauthorizedResponseIsReturned()
    {
        // Given
        const string accessToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxZTA5Y2EyOS0yOTEwLTRmNTQtODAwMi0yZDllMDYzMDkwYzYiLCJ1bmlxdWVfbmFtZSI6ImludmFsaWQuaW52YWxpZEBleGFtcGxlLmNvbSIsImp0aSI6Ijg3MDM5ZTJmLTg2N2UtNDA5ZC1iYjYwLWU4ZGFiYzg0ZjUyZCIsImlhdCI6MTcyODM4OTM5MSwiZXhwIjoxNzI4MzkyOTkxLCJpc3MiOiJTTUFJQVgiLCJhdWQiOiJTb21lQXVkaWVuY2UifQ.16NkwPXPOzeTJNuvSSsIX1fi_F1NZV7_EmiAFikXNYU";
        const string refreshToken = "90502660-ebbb-405a-9c93-0bd4e9c2ba41-714b9f49-e7b9-4090-8e84-61fdbe8e9f6e";
        var tokenDto = new TokenDto(accessToken, refreshToken);
        var httpContent = new StringContent(JsonConvert.SerializeObject(tokenDto), Encoding.UTF8,
            "application/json");

        // When
        var response = await _httpClient.PostAsync($"{BaseUrl}/refresh", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenValidAccessTokenAndValidRefreshToken_WhenLogout_ThenOkIsReturnedAndRefreshTokenIsInvalidated()
    {
        // Given
        const string accessToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzYzA3MDY1YS1iOTY0LTQ0YTktOWNkZi1mYmQ0OWQ3NTVlYTciLCJ1bmlxdWVfbmFtZSI6ImpvaG4uZG9lQGV4YW1wbGUuY29tIiwianRpIjoiMTlmNzdiMmUtZTQ4NS00MDMxLTg1MDYtNjJmNmQzYjY5ZTRkIiwiaWF0IjoxNzI4Mzg5MzkxLCJleHAiOjE3MjgzOTI5OTEsImlzcyI6IlNNQUlBWCIsImF1ZCI6IlNvbWVBdWRpZW5jZSJ9.chHm391Tbcwo-Adq3QPPQses9NJuyUzM0vMjQUR6FmA";
        const string refreshToken = "4dffb63c-581d-4588-8b4b-4b075f17d015-abcb30f4-5f32-4fbb-80c4-99cea98273ca";
        var tokenDto = new TokenDto(accessToken, refreshToken);

        var httpContent = new StringContent(JsonConvert.SerializeObject(tokenDto), Encoding.UTF8,
            "application/json");
        
        // When
        var response = await _httpClient.PostAsync($"{BaseUrl}/logout", httpContent);
        
        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var invalidatedRefreshToken = await _applicationDbContext.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(rt => rt.Token.Equals(refreshToken));
        Assert.That(invalidatedRefreshToken, Is.Not.Null);
        Assert.That(invalidatedRefreshToken.IsValid, Is.False);
    }
}