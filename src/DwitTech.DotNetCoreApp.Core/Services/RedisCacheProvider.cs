using DwitTech.CacheLibrary.Core.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.CacheLibrary.Core.Services
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCacheProvider(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }

        public T Get<T>(string key)
        {
            var cachedValue = _database.StringGet(key);
            if (!cachedValue.IsNull)
            {
                return Deserialize<T>(cachedValue);
            }
            return default;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var serializedValue = Serialize(value);
            _database.StringSet(key, serializedValue, expiration);
        }

        public T GetOrSet<T>(string key, Func<T> valueFactory, TimeSpan expiration)
        {
            // Try to get the value from the cache
            var cachedValue = Get<T>(key);

            if (cachedValue != null)
            {
                return cachedValue;
            }

            // If not found in cache, generate the value using the valueFactory function
            var newValue = valueFactory();

            // Set the value in the cache with the specified expiration
            Set(key, newValue, expiration);

            return newValue;
        }


        public void Remove(string key)
        {
            _database.KeyDelete(key);
        }

        private T Deserialize<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (JsonException ex)
            {
                // Handle deserialization errors (e.g., invalid JSON)
                throw new Exception("Error deserializing cached value", ex);
            }
        }

        private string Serialize<T>(T value)
        {
            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch (JsonException ex)
            {
                // Handle serialization errors
                throw new Exception("Error serializing value for caching", ex);
            }
        }

    }
}
