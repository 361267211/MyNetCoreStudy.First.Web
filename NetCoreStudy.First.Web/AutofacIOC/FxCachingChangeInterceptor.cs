using Castle.DynamicProxy;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using NetCoreStudy.First.Web.FxAttribute;
using System.Linq;

namespace NetCoreStudy.First.Web.AutofacIOC
{
    public class FxCachingChangeInterceptor : IInterceptor
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private protected CachingChangeAttribute _cachingAttribute;

        public FxCachingChangeInterceptor(IDistributedCache cache, IConnectionMultiplexer connectionMultiplexer)
        {
            _cache = cache;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async void Intercept(IInvocation invocation)
        {
            //对当前方法的特性验证
            _cachingAttribute = GetQCachingAttributeInfo(invocation.MethodInvocationTarget ?? invocation.Method);
            if (_cachingAttribute == null)
            {
                invocation.Proceed();//直接执行被拦截方法
                return;
            }
            bool isDefAsync = Attribute.IsDefined(invocation.MethodInvocationTarget, typeof(AsyncStateMachineAttribute));

            if (isDefAsync)
            {
                invocation.Proceed();//直接执行被拦截方法

                var result = invocation.ReturnValue;
                if (result is Task)
                {
                    await Task.WhenAll(result as Task);
                }
            }
            else
            {
                invocation.Proceed();//直接执行被拦截方法
            }
            var lstKey = GetKeysByPrefixQuery(_cachingAttribute.ResourceName);
            foreach (var key in lstKey)
            {
                _cache.Remove(key);
            }

        }
        /// <summary>
        /// 前缀查询redis Key
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public List<string> GetKeysByPrefixQuery(string prefix)
        {
            List<string> lstKeys = new List<string>();
            //获取db
            var db = _connectionMultiplexer.GetDatabase(1);
            //遍历集群内服务器
            foreach (var endPoint in _connectionMultiplexer.GetEndPoints())
            {
                //获取指定服务器
                var server = _connectionMultiplexer.GetServer(endPoint);
                //在指定服务器上使用 keys 或者 scan 命令来遍历key
                foreach (var key in server.Keys(pattern: $"*:Resource:{prefix}:*"))
                {
                    //获取key                    
                    lstKeys.Add(key);
                }
            }
            return lstKeys;
        }

        private CachingChangeAttribute GetQCachingAttributeInfo(MethodInfo method)
        {
            return method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingChangeAttribute)) as CachingChangeAttribute;
        }
    }
}
