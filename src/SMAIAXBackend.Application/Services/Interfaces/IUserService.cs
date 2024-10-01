using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IUserService
{
    Task Register(RegisterDto registerDto);
}