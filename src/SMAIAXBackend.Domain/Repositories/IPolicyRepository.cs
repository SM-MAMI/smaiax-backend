using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Repositories;

public interface IPolicyRepository
{
    PolicyId NextIdentity();
    Task AddAsync(Policy policy);
}