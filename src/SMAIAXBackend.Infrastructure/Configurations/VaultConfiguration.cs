namespace SMAIAXBackend.Infrastructure.Configurations;

public class VaultConfiguration
{
    public required string Address { get; init; }
    public required string Token { get; init; }
    // We need to specify the database host here because when running the application locally
    // the database host for the application is localhost.
    // Vault is running in a container and needs the container name of the database host.
    public required string DatabaseHost { get; init; }
    public required int DatabasePort { get; init; }
    public required string CredentialsDefaultTimeToLive { get; init; }
    public required string CredentialsMaximumTimeToLive { get; init; }
}