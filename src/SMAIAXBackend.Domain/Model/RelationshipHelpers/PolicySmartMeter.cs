using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.RelationshipHelpers;

public class PolicySmartMeter(PolicyId policyId, SmartMeterId smartMeterId)
{
    public PolicyId PolicyId { get; private set; } = policyId;
    public SmartMeterId SmartMeterId { get; private set; } = smartMeterId;
}