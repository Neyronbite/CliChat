using Business.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    //TODO change this to non static
    public static class RateLimitingService
    {
        private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        // TODO use configuration
        private static int RequestLimit = 10;
        private static int TimeWindowInSeconds = 7;
        private static int BanTimeInMinutes = 15;

        private static object _locker = new object();

        public static bool IsRateLimitedAsync(string ipAddress)
        {
            var cacheKey = $"rate_limit_{ipAddress}";

            lock (_locker)
            {
                if (_cache.TryGetValue(cacheKey, out RateLimitInfo rateLimitInfo))
                {
                    // Check if the IP is currently banned
                    if (rateLimitInfo.IsBanned && rateLimitInfo.BanUntil > DateTime.UtcNow)
                    {
                        return true; // Banned
                    }

                    // If requests are within the allowed limit
                    if (rateLimitInfo.RequestCount < RequestLimit)
                    {
                        return false;
                    }
                    else
                    {
                        // Exceeded rate limit, ban the IP
                        rateLimitInfo.IsBanned = true;
                        rateLimitInfo.BanUntil = DateTime.UtcNow.AddMinutes(BanTimeInMinutes);
                        _cache.Set(cacheKey, rateLimitInfo, TimeSpan.FromMinutes(BanTimeInMinutes));

                        return true;
                    }
                }

                // If the IP address is not in the cache, allow the request
                var newRateLimitInfo = new RateLimitInfo
                {
                    RequestCount = 1,
                    LastRequestTime = DateTime.UtcNow,
                    IsBanned = false
                };

                _cache.Set(cacheKey, newRateLimitInfo, TimeSpan.FromSeconds(TimeWindowInSeconds));

                return false;
            }
        }

        public static void RecordRequest(string ipAddress)
        {
            var cacheKey = $"rate_limit_{ipAddress}";

            lock (_locker)
            {
                if (_cache.TryGetValue(cacheKey, out RateLimitInfo rateLimitInfo))
                {
                    // Check if the request is within the time window
                    if (DateTime.UtcNow.Subtract(rateLimitInfo.LastRequestTime).TotalSeconds <= TimeWindowInSeconds)
                    {
                        rateLimitInfo.RequestCount++;
                    }
                    else
                    {
                        // Reset the count if the time window has passed
                        rateLimitInfo.RequestCount = 1;
                    }

                    rateLimitInfo.LastRequestTime = DateTime.UtcNow;

                    // Update the cache
                    _cache.Set(cacheKey, rateLimitInfo, TimeSpan.FromSeconds(TimeWindowInSeconds));
                }
            }
        }
    }
}
