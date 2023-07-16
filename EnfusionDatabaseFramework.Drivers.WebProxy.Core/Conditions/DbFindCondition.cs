using System.Text.Json.Serialization;

namespace EnfusionDatabaseFramework.Drivers.WebProxy.Core.Conditions;


[JsonPolymorphic(TypeDiscriminatorPropertyName = TypeDiscriminator)]
[JsonDerivedType(typeof(DbFindAnd), "DbFindAnd")]
[JsonDerivedType(typeof(DbFindOr), "DbFindOr")]
[JsonDerivedType(typeof(DbFindCheckFieldNullOrDefault), "DbFindCheckFieldNullOrDefault")]
[JsonDerivedType(typeof(DbFindCompareFieldValues<int>), "DbFindCompareFieldValues<int>")]
[JsonDerivedType(typeof(DbFindCompareFieldValues<double>), "DbFindCompareFieldValues<float>")]
[JsonDerivedType(typeof(DbFindCompareFieldValues<bool>), "DbFindCompareFieldValues<bool>")]
[JsonDerivedType(typeof(DbFindCompareFieldValues<string>), "DbFindCompareFieldValues<string>")]
[JsonDerivedType(typeof(DbFindCompareFieldValues<Vector>), "DbFindCompareFieldValues<vector>")]
[JsonDerivedType(typeof(DbFindCompareFieldValues<Typename>), "DbFindCompareFieldValues<typename>")]
[JsonDerivedType(typeof(DbFindCompareFieldValues<List<int>>), "DbFindCompareFieldValues<array<int>>")]
[JsonDerivedType(typeof(DbFindCompareFieldValues<List<double>>), "DbFindCompareFieldValues<array<float>>")]
[JsonDerivedType(typeof(DbFindCompareFieldValues<List<bool>>), "DbFindCompareFieldValues<array<bool>>")]
[JsonDerivedType(typeof(DbFindCompareFieldValues<List<string>>), "DbFindCompareFieldValues<array<string>>")]
[JsonDerivedType(typeof(DbFindCompareFieldValues<List<Vector>>), "DbFindCompareFieldValues<array<vector>>")]
[JsonDerivedType(typeof(DbFindCompareFieldValues<List<Typename>>), "DbFindCompareFieldValues<array<typename>>")]
public class DbFindCondition
{
    public const string TypeDiscriminator = "_type";
}
