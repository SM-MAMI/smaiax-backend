namespace SMAIAXBackend.Application.Exceptions;

public class SmartMeterNotFoundException(Guid smartMeterId, Guid userId) : Exception
{
    public override string Message { get; } = $"Smart meter with id '{smartMeterId} not found for user with id '{userId}'.";
}