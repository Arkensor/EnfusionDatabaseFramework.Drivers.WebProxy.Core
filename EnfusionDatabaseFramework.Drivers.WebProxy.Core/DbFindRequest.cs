using EnfusionDatabaseFramework.Drivers.WebProxy.Core.Conditions;

namespace EnfusionDatabaseFramework.Drivers.WebProxy.Core;

public class DbFindRequest
{
    public DbFindCondition? Condition { get; set; }

    public List<List<string>>? OrderBy { get; set; }

    public int? Limit { get; set; }

    public int? Offset { get; set; }
}
