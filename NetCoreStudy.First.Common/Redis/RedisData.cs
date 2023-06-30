using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Common.Redis
{
    public class RedisData<T>
    {
        public TimeSpan LogicExpireTimeSpan   =>TimeSpan.FromTicks(DateTime.Now.AddSeconds(100).Ticks);
        public T Data   { get; set; }

        public RedisData(T data )
        {
            Data = data;
        }
    }
}
