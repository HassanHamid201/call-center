namespace Application.Interfaces;

/// <summary>
/// Defines the contract for cache operations using the cache-aside pattern
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a value from cache. If not found, uses the factory to create and cache it.
    /// </summary>
    /// <typeparam name="T">The type of value to cache</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="factory">Function to create the value if not in cache</param>
    /// <param name="expiration">Optional expiration time</param>
    /// <returns>The cached or newly created value</returns>
    Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);

    /// <summary>
    /// Removes a value from cache
    /// </summary>
    /// <param name="key">The cache key</param>
    Task RemoveAsync(string key);

    /// <summary>
    /// Removes multiple values from cache by pattern
    /// </summary>
    /// <param name="pattern">The cache key pattern</param>
    Task RemoveByPatternAsync(string pattern);
}
