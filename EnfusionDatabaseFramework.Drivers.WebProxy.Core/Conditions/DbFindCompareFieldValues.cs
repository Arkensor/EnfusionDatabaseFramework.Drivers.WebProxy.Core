namespace EnfusionDatabaseFramework.Drivers.WebProxy.Core.Conditions;

public class DbFindCompareFieldCondition : DbFindFieldCondition
{
    public bool StringsInvariant { get; set; }

    public bool StringsPartialMatches { get; set; }

    public DbFindOperator ComparisonOperator { get; set; }
}

public class DbFindCompareFieldValues<T> : DbFindCompareFieldCondition
{
    public required List<T> ComparisonValues { get; set; }
}
