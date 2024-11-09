using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IPolicyRequestCreateService
{
    Task<Guid> CreatePolicyRequestAsync(PolicyRequestCreateDto policyRequestCreateDto);
}