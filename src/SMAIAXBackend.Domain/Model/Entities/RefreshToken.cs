using System.Diagnostics.CodeAnalysis;

using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class RefreshToken : IEquatable<RefreshToken>
{
    public RefreshTokenId Id { get; }

    public UserId UserId { get; }

    public string JwtTokenId { get; }

    public string Token { get; }

    public bool IsValid { get; private set; }

    public DateTime ExpiresAt { get; }

    public static RefreshToken Create(
        RefreshTokenId id,
        UserId userId,
        string jwtTokenId,
        string token,
        bool isValid,
        DateTime expiresAt)
    {
        return new RefreshToken(id, userId, jwtTokenId, token, isValid, expiresAt);
    }

    // Needed by EF Core
    [ExcludeFromCodeCoverage]
    private RefreshToken()
    {
    }

    private RefreshToken(
        RefreshTokenId id,
        UserId userId,
        string jwtTokenId,
        string token,
        bool isValid,
        DateTime expiresAt)
    {
        Id = id;
        UserId = userId;
        JwtTokenId = jwtTokenId;
        Token = token;
        IsValid = isValid;
        ExpiresAt = expiresAt;
    }

    public void Invalidate()
    {
        IsValid = false;
    }

    [ExcludeFromCodeCoverage]
    public bool Equals(RefreshToken? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id);
    }

    [ExcludeFromCodeCoverage]
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((RefreshToken)obj);
    }

    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}