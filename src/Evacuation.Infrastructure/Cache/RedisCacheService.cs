using Evacuation.Infrastructure.Cache.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Evacuation.Infrastructure.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database; // This is the Redis database instance
        private readonly IConnectionMultiplexer _redis; // This is the connection to the Redis server

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = redis.GetDatabase();
        }
        public async Task ClearCacheAsync()
        {
            var endpoints = _redis.GetEndPoints(); // Get all endpoints (servers) in the Redis cluster
            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint); // Get the server instance for each endpoint
                if (server.IsConnected)
                {
                    await server.FlushDatabaseAsync(); // Flush the database on the server
                }
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _database.StringGetAsync(key);
            if (json.IsNullOrEmpty)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(json!);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiration);
        }
    }
}
