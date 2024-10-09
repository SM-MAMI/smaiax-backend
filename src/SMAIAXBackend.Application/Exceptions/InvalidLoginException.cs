namespace SMAIAXBackend.Application.Exceptions;

public class InvalidLoginException : Exception
{
    public override string Message => "Username or password is wrong.";
}