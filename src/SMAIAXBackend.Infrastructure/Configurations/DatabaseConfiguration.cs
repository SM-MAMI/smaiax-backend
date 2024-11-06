namespace SMAIAXBackend.Infrastructure.Configurations;

public class DatabaseConfiguration
{
    public required string Host { get; init; }
    public required string Port { get; init; }
    public required string SuperUsername { get; init; }
    public required string SuperUserPassword { get; init; }
    public required string MainDatabase { get; init; }
}