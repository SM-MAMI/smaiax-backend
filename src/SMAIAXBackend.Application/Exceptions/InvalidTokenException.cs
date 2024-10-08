namespace SMAIAXBackend.Application.Exceptions;

public class InvalidTokenException : Exception
{
    public override string Message => "The token is invalid.";
}