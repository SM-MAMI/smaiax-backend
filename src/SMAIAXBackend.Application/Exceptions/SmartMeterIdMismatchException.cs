namespace SMAIAXBackend.Application.Exceptions;

public class SmartMeterIdMismatchException(Guid smartMeterIdExpected, Guid smartMeterIdActual) : Exception
{
    public override string Message { get; } = $"SmartMeterId `{smartMeterIdExpected}` in the path does not match the SmartMeterId `{smartMeterIdActual}` in the body.";
}