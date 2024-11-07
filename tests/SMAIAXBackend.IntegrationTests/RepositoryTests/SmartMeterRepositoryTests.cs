using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.IntegrationTests.RepositoryTests;

[TestFixture]
public class SmartMeterRepositoryTests : TestBase
{
    [Test]
    public async Task GivenSmartMeter_WhenAdd_ThenExpectedSmartMeterIsPersisted()
    {
        // Given
        var smartMeterExpected = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Test",
            new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7")));
        var tenant = Tenant.Create(new TenantId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39")), "johndoe",
            "P@ssw0rd", "tenant_1_db");

        // When
        await _smartMeterRepository.AddAsync(smartMeterExpected, tenant);
        var smartMeterActual = await _tenantDbContext.SmartMeters
            .AsNoTracking()
            .FirstOrDefaultAsync(sm => sm.Id.Equals(smartMeterExpected.Id));

        // Then
        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(smartMeterActual.Id, Is.EqualTo(smartMeterExpected.Id));
            Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterExpected.Name));
            Assert.That(smartMeterActual.UserId, Is.EqualTo(smartMeterExpected.UserId));
        });
    }

    [Test]
    public async Task GivenSmartMetersInRepository_WhenGetSmartMetersByUserId_ThenExpectedSmartMetersAreReturned()
    {
        // Given
        var tenant = Tenant.Create(new TenantId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39")), "johndoe",
            "P@ssw0rd", "tenant_1_db");
        var userId = new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7"));
        var smartMetersExpected = new List<SmartMeter>()
        {
            SmartMeter.Create(new SmartMeterId(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd")), "Smart Meter 1",
                userId),
            SmartMeter.Create(new SmartMeterId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39")), "Smart Meter 2",
                userId)
        };

        // When
        var smartMetersActual = await _smartMeterRepository.GetSmartMetersAsync(tenant);

        // Then
        Assert.That(smartMetersActual, Is.Not.Null);
        Assert.That(smartMetersActual, Has.Count.EqualTo(smartMetersExpected.Count));

        for (int i = 0; i < smartMetersActual.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(smartMetersActual[i].Id, Is.EqualTo(smartMetersExpected[i].Id));
                Assert.That(smartMetersActual[i].Name, Is.EqualTo(smartMetersExpected[i].Name));
                Assert.That(smartMetersActual[i].UserId, Is.EqualTo(smartMetersExpected[i].UserId));
            });
        }
    }

    [Test]
    public async Task GivenSmartMeterInRepository_WhenGetSmartMeterByIdAndUserId_ThenExpectedSmartMeterIsReturned()
    {
        // Given
        var tenant = Tenant.Create(new TenantId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39")), "johndoe",
            "P@ssw0rd", "tenant_1_db");
        var userId = new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7"));
        var smartMeterExpected = SmartMeter.Create(new SmartMeterId(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd")),
            "Smart Meter 1", userId);

        // When
        var smartMeterActual = await _smartMeterRepository.GetSmartMeterByIdAsync(smartMeterExpected.Id, tenant);

        // Then
        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(smartMeterActual.Id, Is.EqualTo(smartMeterExpected.Id));
            Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterExpected.Name));
            Assert.That(smartMeterActual.UserId, Is.EqualTo(smartMeterExpected.UserId));
        });
    }

    [Test]
    public async Task GivenSmartMeterInRepository_WhenUpdate_ThenExpectedSmartMeterIsUpdated()
    {
        // Given
        var tenant = Tenant.Create(new TenantId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39")), "johndoe",
            "P@ssw0rd", "tenant_1_db");
        const string name = "Smart Meter 1";
        var smartMeterExpected = SmartMeter.Create(new SmartMeterId(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd")),
            "Smart Meter 1 Updated", new UserId(Guid.Parse("3c07065a-b964-44a9-9cdf-fbd49d755ea7")));

        // When
        await _smartMeterRepository.UpdateAsync(smartMeterExpected, tenant);

        // Then
        var smartMeterActual = await _tenantDbContext.SmartMeters
            .AsNoTracking()
            .FirstOrDefaultAsync(sm => sm.Id.Equals(smartMeterExpected.Id));
        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterExpected.Name));
            Assert.That(smartMeterActual.Name, Is.Not.EqualTo(name));
        });
    }
}