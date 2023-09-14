using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.CacheLibrary.Core.Interfaces
{
    public interface ICacheProvider
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan expiration);
        T GetOrSet<T>(string key, Func<T> valueFactory, TimeSpan expiration);

        void Remove(string key);
    }
}
