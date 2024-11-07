namespace SMAIAXBackend.Application.Exceptions;

public class TenantNotFoundException(Guid tenantId, Guid userId) : Exception
{
    public override string Message { get; } = $"Tenant with id '{tenantId}' not found for user with id '{userId}'.";
}