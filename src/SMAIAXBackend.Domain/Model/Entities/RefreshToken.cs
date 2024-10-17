using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public class RefreshToken : IEqualityComparer<RefreshTokenId>
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

    public bool Equals(RefreshTokenId? x, RefreshTokenId? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        return x.Id.Equals(y.Id);
    }

    public int GetHashCode(RefreshTokenId obj)
    {
        return obj.Id.GetHashCode();
    }
}