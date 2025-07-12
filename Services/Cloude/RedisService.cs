﻿using E_learning.Model.cloudeDB;
using StackExchange.Redis;

namespace E_learning.Services.Cloude
{
    public class RedisService
    {
        private readonly IDatabase _redis;
        private readonly ILogger<RedisService> _logger;
        public RedisService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisService> logger)
        {
            _redis = connectionMultiplexer.GetDatabase();
            _logger = logger;
        }

        public async Task<bool> SetAsync(RedisModel redis)
        {
            int ttLSeconds = 600 + new Random().Next(0, 300); 
            if (redis == null || string.IsNullOrEmpty(redis.key) || string.IsNullOrEmpty(redis.value))
            {
                _logger.LogWarning("RedisModel is null or has empty key/value.");
                return false;
            }
            var result = await _redis.StringSetAsync(redis.key, redis.value, TimeSpan.FromSeconds(ttLSeconds));
            if (!result)
            {
                _logger.LogError("Failed to set value in Redis for key: {Key}", redis.key);
            }
            else
            {
                _logger.LogInformation("Successfully set value in Redis for key: {Key}", redis.key);
            }
            return result;
        }
        public async Task<string?> GetAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning("Key is null or empty.");
                return null;
            }
            return await _redis.StringGetAsync(key);
        }
    }
}
