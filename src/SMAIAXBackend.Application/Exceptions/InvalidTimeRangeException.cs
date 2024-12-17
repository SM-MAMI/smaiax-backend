namespace SMAIAXBackend.Application.Exceptions;

public class InvalidTimeRangeException : Exception
{
    public override string Message => "StartAt must be before or equal to endAt.";
}