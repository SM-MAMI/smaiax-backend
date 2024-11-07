namespace SMAIAXBackend.Application.Exceptions;

public class SmartMeterNotFoundException(Guid smartMeterId) : Exception
{
    public override string Message { get; } = $"Smart meter with id '{smartMeterId} not found.";
}