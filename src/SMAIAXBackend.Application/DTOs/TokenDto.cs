using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class TokenDto(string accessToken, string refreshToken)
{
    [Required]
    public string AccessToken { get; } = accessToken;

    [Required]
    public string RefreshToken { get; } = refreshToken;
}