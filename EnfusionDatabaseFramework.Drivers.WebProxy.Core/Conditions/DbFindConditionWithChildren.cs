namespace EnfusionDatabaseFramework.Drivers.WebProxy.Core.Conditions;

public class DbFindConditionWithChildren : DbFindCondition
{
    public required List<DbFindCondition> Conditions { get; set; }
}
