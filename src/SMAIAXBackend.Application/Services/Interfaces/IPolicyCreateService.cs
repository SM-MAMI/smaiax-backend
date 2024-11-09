using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IPolicyCreateService
{
    Task<Guid> CreatePolicyAsync(PolicyCreateDto policyCreateDto);
}