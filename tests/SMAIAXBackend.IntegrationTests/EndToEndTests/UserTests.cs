using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Domain.Model.ValueObjects;

namespace SMAIAXBackend.IntegrationTests.EndToEndTests;

[TestFixture]
public class UserTests : TestBase
{
    private const string BaseUrl = "/api/users";

    [Test]
    public async Task GivenUserInformation_WhenRegister_ThenDomainUserAndIdentityUserAreCreated()
    {
        // Given
        var registerDto = new RegisterDto("user@example.com", "P@ssw0rd", new Name("John", "Doe"),
            new Address("123 Main St", "Anytown", "CA", "12345", "USA"));

        var httpContent = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8,
            "application/json");

        // When
        var response = await HttpClient.PostAsync($"{BaseUrl}/register", httpContent);
        var responseContent = await response.Content.ReadAsStringAsync();
        var id = JsonConvert.DeserializeObject<Guid>(responseContent);

        // Then
        response.EnsureSuccessStatusCode();
        Assert.That(responseContent, Is.Not.Null);

        var identityUser = await UserStoreDbContext.Users
            .SingleOrDefaultAsync(u => u.Id == id.ToString());
        var domainUser = await UserStoreDbContext.DomainUsers
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
            Assert.That(domainUser.Name, Is.EqualTo(registerDto.Name));
            Assert.That(domainUser.Address, Is.EqualTo(registerDto.Address));
        });
    }

    [Test]
    public async Task GivenInvalidUserInformation_WhenRegister_ThenErrorResponseIsReturned()
    {
        // Given
        var registerDto = new RegisterDto("user@example.com", "Passw0rd", new Name("John", "Doe"),
            new Address("123 Main St", "Anytown", "CA", "12345", "USA"));

        var httpContent = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8,
            "application/json");

        // When
        var response = await HttpClient.PostAsync($"{BaseUrl}/register", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Does.Contain("Registration Error"));
        Assert.That(responseContent, Does.Contain("Registration failed with the following errors: Passwords must have at least one non alphanumeric character."));
    }

    [Test]
    public async Task GivenValidUsernameAndPassword_WhenLogin_ThenAccessTokenIsReturned()
    {
        // Given
        var loginDto = new LoginDto("john.doe@example.com", "P@ssw0rd");
        var httpContent = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8,
            "application/json");
        
        // When
        var response = await HttpClient.PostAsync($"{BaseUrl}/login", httpContent);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Then
        response.EnsureSuccessStatusCode();
        Assert.That(responseContent, Is.Not.Null);
    }
    
    [Test]
    public async Task GivenInvalidUsernameAndValidPassword_WhenLogin_ThenErrorResponseIsReturned()
    {
        // Given
        var loginDto = new LoginDto("john.invalid@example.com", "P@ssw0rd");
        var httpContent = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8,
            "application/json");
        
        // When
        var response = await HttpClient.PostAsync($"{BaseUrl}/login", httpContent);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
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
        var response = await HttpClient.PostAsync($"{BaseUrl}/login", httpContent);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Does.Contain("Unauthorized"));
        Assert.That(responseContent, Does.Contain("Username or password is wrong"));
    }
}