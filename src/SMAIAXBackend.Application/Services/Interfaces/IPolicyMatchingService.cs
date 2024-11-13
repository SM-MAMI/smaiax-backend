using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IPolicyMatchingService
{
    Task<List<PolicyDto>> GetMatchingPoliciesAsync(Guid policyRequestId);
}