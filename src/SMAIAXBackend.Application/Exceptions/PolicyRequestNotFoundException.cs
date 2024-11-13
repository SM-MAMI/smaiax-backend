namespace SMAIAXBackend.Application.Exceptions;

public class PolicyRequestNotFoundException(Guid id) : Exception
{
    public override string Message { get; } = $"Policy request with id '{id}' not found.";
}