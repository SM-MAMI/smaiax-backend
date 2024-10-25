namespace SMAIAXBackend.Application.Exceptions;

public class UserNotFoundException(Guid id) : Exception
{
    public override string Message { get; } = $"User with id '{id}' not found.";
}