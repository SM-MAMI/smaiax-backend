using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.IntegrationTests.RepositoryTests;

public class UserRepositoryTests : TestBase
{
    [Test]
    public async Task GivenUser_WhenAdd_ThenExpectedUserIsPersisted()
    {
        // Given
        var tenantId = new TenantId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39"));
        var userExpected = User.Create(new UserId(Guid.NewGuid()), new Name("Test", "Test"), "test", "test@test.com", tenantId);

        // When
        await _userRepository.AddAsync(userExpected);
        var userActual = await _applicationDbContext.DomainUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id.Equals(userExpected.Id));

        // Then
        Assert.That(userActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(userActual.Id, Is.EqualTo(userExpected.Id));
            Assert.That(userActual.Name, Is.EqualTo(userExpected.Name));
            Assert.That(userActual.Email, Is.EqualTo(userExpected.Email));
        });
    }

    [Test]
    public async Task GivenUserId_WhenGetUserById_ThenExpectedUserIsReturned()
    {
        // Given
        var tenantId = new TenantId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39"));
        var userExpected = User.Create(new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7")),
            new Name("John", "Doe"), "test", "john.doe@example.com", tenantId);

        // When
        var userActual = await _userRepository.GetUserByIdAsync(userExpected.Id);

        // Then
        Assert.That(userActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(userActual.Id, Is.EqualTo(userExpected.Id));
            Assert.That(userActual.Name, Is.EqualTo(userExpected.Name));
            Assert.That(userActual.Email, Is.EqualTo(userExpected.Email));
        });
    }
}