using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.Application.Exceptions;

public class InsufficientLocationDataException(Guid smartMeterId, LocationResolution locationResolution) : Exception
{
    public override string Message { get; } =
        $"SmartMeter with id {smartMeterId} does not have sufficient location data for resolution {locationResolution}.";
}