using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IUserService
{
    Task<Guid> RegisterAsync(RegisterDto registerDto);
}