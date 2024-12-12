using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class MeasurementListService(
    IMeasurementRepository measurementRepository,
    ISmartMeterRepository smartMeterRepository) : IMeasurementListService
{
    public async Task<List<MeasurementRawDto>> GetFilteredMeasurementsByTenantAndSmartMeterAsync(
        Guid smartMeterId, DateTime? startAt = null,
        DateTime? endAt = null)
    {
        SmartMeterId smId = new SmartMeterId(smartMeterId);
        SmartMeter? smartMeter = await smartMeterRepository.GetSmartMeterByIdAsync(smId);
        if (smartMeter == null)
        {
            throw new SmartMeterNotFoundException(smartMeterId);
        }

        var measurements =
            await measurementRepository.GetMeasurementsByTenantAndSmartMeterAsync(smId, startAt, endAt);
        return measurements.Select(MeasurementRawDto.FromMeasurement).ToList();
    }
}