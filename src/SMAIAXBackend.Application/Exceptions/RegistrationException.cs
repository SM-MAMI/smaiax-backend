namespace SMAIAXBackend.Application.Exceptions;

public class RegistrationException(string errorMessages) : Exception
{
    public override string Message { get; } = $"Registration failed with the following errors: {errorMessages}";
}