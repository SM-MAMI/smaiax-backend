namespace SMAIAXBackend.Application.Exceptions;

public class SmartMeterOwnershipException(Guid smartMeterId, Guid userId) : Exception
{
    public override string Message { get; } = $"SmartMeter with id {smartMeterId} does not belong to user {userId}";
}