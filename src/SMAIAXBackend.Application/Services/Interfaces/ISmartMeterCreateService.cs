using System.Security.Claims;

using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface ISmartMeterCreateService
{
    Task<Guid> AddSmartMeterAsync(SmartMeterCreateDto smartMeterCreateDto, ClaimsPrincipal userClaimsPrincipal);
}