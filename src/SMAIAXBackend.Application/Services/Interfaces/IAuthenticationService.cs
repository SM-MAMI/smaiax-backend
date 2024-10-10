using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IAuthenticationService
{
    Task<Guid> RegisterAsync(RegisterDto registerDto);
    Task<TokenDto> LoginAsync(LoginDto loginDto);
    Task<TokenDto> RefreshTokensAsync(TokenDto tokenDto);
    Task LogoutAsync(string refreshToken);
}