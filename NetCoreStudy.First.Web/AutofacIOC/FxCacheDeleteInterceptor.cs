using Castle.DynamicProxy;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Caching.Distributed;
using System;

namespace NetCoreStudy.First.Web.AutofacIOC
{

    /// <summary>
    /// 使用autofac做的缓存装饰器:改装饰器用于删除缓存
    /// </summary>
    public class FxCacheDeleteInterceptor : IInterceptor
    {
        private readonly IDistributedCache _cache;

        public FxCacheDeleteInterceptor(IDistributedCache cache)
        {
            _cache = cache;
        }

        public FxCacheDeleteInterceptor()
        {
        }

        public async void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            Console.WriteLine($"{invocation.Method.Name}'s result:{invocation.ReturnValue.ToJsonString()}");//print return value

            //delete 
            _cache.Remove("zzq");
            //invocation.ReturnValue.ToJsonString();
        }
    }
}
