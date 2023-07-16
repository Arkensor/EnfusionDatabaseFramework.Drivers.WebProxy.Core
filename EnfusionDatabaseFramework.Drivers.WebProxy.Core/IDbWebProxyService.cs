namespace EnfusionDatabaseFramework.Drivers.WebProxy.Core;

public interface IDbWebProxyService
{
    Task AddOrUpdateAsync(string database, string collection, Guid id, string data, CancellationToken cancellationToken);

    Task RemoveAsync(string database, string collection, Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<string>> FindAllAsync(string database, string collection, DbFindRequest findRequest, CancellationToken cancellationToken);
}
