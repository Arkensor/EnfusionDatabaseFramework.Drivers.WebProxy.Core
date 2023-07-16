namespace EnfusionDatabaseFramework.Drivers.WebProxy.Core.Conditions;

public class DbFindCheckFieldNullOrDefault : DbFindFieldCondition
{
    public bool ShouldBeNullOrDefault { get; set; }
}
