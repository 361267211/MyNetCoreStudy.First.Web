using StackExchange.Redis;
using System;

namespace NetCoreStudy.First.Web.AutofacIOC
{
    public class RedisMutex
    {
        private readonly IDatabase _redisDatabase;
        private readonly string _lockKey;
        private readonly string _lockValue;

        public RedisMutex(IDatabase redisDatabase, string lockKey, string lockValue)
        {
            _redisDatabase = redisDatabase;
            _lockKey = lockKey;
            _lockValue = lockValue;
        }

        public bool AcquireLock()
        {
            return _redisDatabase.StringSet(_lockKey, _lockValue, TimeSpan.FromSeconds(10), When.NotExists);
        }

        public void ReleaseLock()
        {
            _redisDatabase.KeyDelete(_lockKey);
        }
    }
}
