using Moq;

using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterListServiceTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<IUserValidationService> _userValidationServiceMock;
    private SmartMeterListService _smartMeterListService;

    [SetUp]
    public void Setup()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _userValidationServiceMock = new Mock<IUserValidationService>();
        _smartMeterListService =
            new SmartMeterListService(_smartMeterRepositoryMock.Object, _userValidationServiceMock.Object);
    }

    [Test]
    public async Task GivenUserId_WhenGetSmartMetersByUserId_ThenExpectedSmartMetersAreReturned()
    {
        // Given
        var userId = new UserId(Guid.NewGuid());
        var smartMetersExpected = new List<SmartMeter>()
        {
            SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 1", userId),
            SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Smart Meter 2", userId)
        };


        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userId.Id.ToString()))
            .ReturnsAsync(userId);
        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMetersByUserIdAsync(userId))
            .ReturnsAsync(smartMetersExpected);

        // When
        var smartMetersActual = await _smartMeterListService.GetSmartMetersByUserIdAsync(userId.Id.ToString());

        // Then
        Assert.That(smartMetersActual, Is.Not.Null);
        Assert.That(smartMetersActual, Has.Count.EqualTo(smartMetersExpected.Count));

        for (int i = 0; i < smartMetersActual.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(smartMetersActual[i].Id, Is.EqualTo(smartMetersExpected[i].Id.Id));
                Assert.That(smartMetersActual[i].Name, Is.EqualTo(smartMetersExpected[i].Name));
                Assert.That(smartMetersActual[i].MetadataCount, Is.EqualTo(smartMetersExpected[i].Metadata.Count));
                Assert.That(smartMetersActual[i].PolicyCount, Is.EqualTo(smartMetersExpected[i].Policies.Count));
            });
        }
    }
}