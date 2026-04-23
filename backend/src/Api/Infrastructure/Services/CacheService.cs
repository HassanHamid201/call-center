using Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Services;

/// <summary>
/// Redis-based cache service implementing the cache-aside pattern
/// </summary>
public class CacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connection;
    private readonly IDatabase _database;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(5);

    public CacheService(IConnectionMultiplexer connection)
    {
        _connection = connection;
        _database = connection.GetDatabase();
    }

    /// <inheritdoc/>
    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var cachedValue = await _database.StringGetAsync(key);
        if (!cachedValue.IsNullOrEmpty)
        {
            return JsonSerializer.Deserialize<T>(cachedValue!);
        }

        var value = await factory();
        if (value is not null)
        {
            await _database.StringSetAsync(
                key,
                JsonSerializer.Serialize(value),
                expiration ?? _defaultExpiration);
        }

        return value;
    }

    /// <inheritdoc/>
    public Task RemoveAsync(string key)
    {
        return _database.KeyDeleteAsync(key);
    }

    /// <inheritdoc/>
    public async Task RemoveByPatternAsync(string pattern)
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern).ToArray();

        if (keys.Length > 0)
        {
            await _database.KeyDeleteAsync(keys);
        }
    }
}
