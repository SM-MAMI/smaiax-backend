using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class MeasurementListService(
    IMeasurementRepository measurementRepository,
    ISmartMeterRepository smartMeterRepository) : IMeasurementListService
{
    public async Task<List<MeasurementRawDto>> GetMeasurementsBySmartMeterAsync(
        Guid smartMeterId, DateTime startAt, DateTime endAt)
    {
        var smId = new SmartMeterId(smartMeterId);
        var smartMeter = await smartMeterRepository.GetSmartMeterByIdAsync(smId);
        if (smartMeter == null)
        {
            throw new SmartMeterNotFoundException(smartMeterId);
        }

        if (endAt < startAt)
        {
            throw new InvalidTimeRangeException();
        }

        var measurements = await measurementRepository.GetMeasurementsBySmartMeterAsync(smId, startAt, endAt);
        return measurements.Select(MeasurementRawDto.FromMeasurement).ToList();
    }
}