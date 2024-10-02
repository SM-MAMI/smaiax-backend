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
        var registerDto = new RegisterDto("user@example.com", "P@ssw0rd", "John", "Doe",
            "123 Main St", "Anytown", "CA", "12345", "USA");

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
            Assert.That(domainUser.Name.FirstName, Is.EqualTo(registerDto.FirstName));
            Assert.That(domainUser.Name.LastName, Is.EqualTo(registerDto.LastName));
            Assert.That(domainUser.Address.Street, Is.EqualTo(registerDto.Street));
            Assert.That(domainUser.Address.City, Is.EqualTo(registerDto.City));
            Assert.That(domainUser.Address.State, Is.EqualTo(registerDto.State));
            Assert.That(domainUser.Address.ZipCode, Is.EqualTo(registerDto.ZipCode));
            Assert.That(domainUser.Address.Country, Is.EqualTo(registerDto.Country));
        });
    }
}