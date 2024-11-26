using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IPolicyRequestCreateService
{
    Task<List<PolicyDto>> CreatePolicyRequestAsync(PolicyRequestCreateDto policyRequestCreateDto);
}