using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Services.Caching
{
    public static class RedisCacheExtensions
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache,
            string recordId,
            T data,
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            options.AbsoluteExpirationRelativeToNow=absoluteExpireTime??TimeSpan.FromSeconds(3600);
            options.SlidingExpiration=unusedExpireTime;

            var jsonData = JsonConvert.SerializeObject(data);

            await cache.SetStringAsync(recordId, jsonData, options);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string rercordId)
        {
            var jsonData = await cache.GetStringAsync(rercordId);

            if (jsonData is null)
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}
