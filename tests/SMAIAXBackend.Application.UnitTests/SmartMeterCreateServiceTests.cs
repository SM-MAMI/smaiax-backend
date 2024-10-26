using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterCreateServiceTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<IUserValidationService> _userValidationServiceMock;
    private SmartMeterCreateService _smartMeterCreateService;

    [SetUp]
    public void Setup()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _userValidationServiceMock = new Mock<IUserValidationService>();
        _smartMeterCreateService = new SmartMeterCreateService(_smartMeterRepositoryMock.Object,
            _userValidationServiceMock.Object);
    }

    [Test]
    public async Task GivenSmartMeterCreateDtoAndExistentUserId_WhenAddSmartMeter_ThenSmartMeterIdIsReturned()
    {
        // Given
        var smartMeterIdExpected = new SmartMeterId(Guid.NewGuid());
        var smartMeterCreateDto = new SmartMeterCreateDto("Test Smart Meter");
        var userId = new UserId(Guid.NewGuid());

        _userValidationServiceMock.Setup(service => service.ValidateUserAsync(userId.Id.ToString())).ReturnsAsync(userId);
        _smartMeterRepositoryMock.Setup(repo => repo.NextIdentity()).Returns(smartMeterIdExpected);

        // When
        var smartMeterIdActual = await _smartMeterCreateService.AddSmartMeterAsync(smartMeterCreateDto, userId.Id.ToString());

        // Then
        Assert.That(smartMeterIdActual, Is.EqualTo(smartMeterIdExpected.Id));
        _smartMeterRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<SmartMeter>()), Times.Once);
    }
}