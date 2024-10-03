namespace SMAIAXBackend.Application.Exceptions;

public class InvalidLoginException : Exception
{
    public override string Message { get; } = "Username or password is wrong.";
}