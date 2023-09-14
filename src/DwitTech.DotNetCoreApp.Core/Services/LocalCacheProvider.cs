using DwitTech.CacheLibrary.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.CacheLibrary.Core.Services
{
    public class LocalCacheProvider : ICacheProvider
    {
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            _cache[key] = value;
        }

        public T GetOrSet<T>(string key, Func<T> valueFactory, TimeSpan expiration)
        {
            var cachedValue = Get<T>(key);
            if (cachedValue != null)
            {
                return cachedValue;
            }

            var newValue = valueFactory();
            Set(key, newValue, expiration);
            return newValue;
        }
        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}

