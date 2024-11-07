using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Repositories;

public interface IPolicyRequestRepository
{
    PolicyRequestId NextIdentity();
    Task AddAsync(PolicyRequest policyRequest);
}