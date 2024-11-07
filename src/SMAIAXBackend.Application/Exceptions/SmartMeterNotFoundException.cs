namespace SMAIAXBackend.Application.Exceptions;

public class SmartMeterNotFoundException(Guid smartMeterId, Guid tenantId) : Exception
{
    public override string Message { get; } = $"Smart meter with id '{smartMeterId} not found for tenant with id '{tenantId}'.";
}