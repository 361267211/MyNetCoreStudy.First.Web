using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using NetCoreStudy.First.Domain;

namespace NetCoreStudy.First.Web.AutofacIOC
{
    /// <summary>
    /// 配置Autofac IOC 容器的模组
    /// </summary>
    public class FelixAutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FelixAutofacCacheInterceptor>();
            builder.RegisterType<UserManagerService>()
                .As<IUserManagerService>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(FelixAutofacCacheInterceptor));
            base.Load(builder);
        }
    }
}
