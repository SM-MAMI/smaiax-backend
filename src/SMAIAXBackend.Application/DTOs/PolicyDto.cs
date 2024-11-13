using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.Application.DTOs;

public class PolicyDto(
    Guid id,
    MeasurementResolution measurementResolution,
    LocationResolution locationResolution,
    decimal price)
{
    public Guid Id { get; set; } = id;
    public MeasurementResolution MeasurementResolution { get; set; } = measurementResolution;
    public LocationResolution LocationResolution { get; set; } = locationResolution;
    public decimal Price { get; set; } = price;

    public static PolicyDto FromPolicy(Policy policy)
    {
        return new PolicyDto(policy.Id.Id, policy.MeasurementResolution, policy.LocationResolution, policy.Price);
    }
}