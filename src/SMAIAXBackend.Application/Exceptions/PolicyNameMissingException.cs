namespace SMAIAXBackend.Application.Exceptions;

public class PolicyNameMissingException : Exception
{
    public override string Message { get; } = "Policy Name missing";
}