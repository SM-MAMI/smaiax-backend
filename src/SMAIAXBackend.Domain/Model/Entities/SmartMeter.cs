using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public class SmartMeter : IEqualityComparer<SmartMeter>
{
    public SmartMeterId Id { get; } = null!;
    public string Name { get; } = null!;

    public static SmartMeter Create(SmartMeterId smartMeterId, string name)
    {
        return new SmartMeter(smartMeterId, name);
    }

    // Needed by EF Core
    private SmartMeter()
    {
    }

    private SmartMeter(SmartMeterId smartMeterId, string name)
    {
        Id = smartMeterId;
        Name = name;
    }

    public bool Equals(SmartMeter? x, SmartMeter? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        return x.Id.Equals(y.Id);
    }

    public int GetHashCode(SmartMeter obj)
    {
        return obj.Id.GetHashCode();
    }
}