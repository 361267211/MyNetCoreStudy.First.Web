using Castle.DynamicProxy;
using Grpc.Core.Interceptors;
using System;

namespace NetCoreStudy.First.Web.AutofacIOC
{

    /// <summary>
    /// 使用autofac做的缓存装饰器
    /// </summary>
    public class FelixAutofacCacheInterceptor : IInterceptor
    {
        public async void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            Console.WriteLine($"{invocation.Method.Name}'s result:{invocation.ReturnValue.ToJsonString()}");//print return value
            //invocation.ReturnValue.ToJsonString();
        }
    }
}
