using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using NetCoreStudy.First.Domain.FxService;

namespace NetCoreStudy.First.Web.AutofacIOC
{
    /// <summary>
    /// 配置Autofac IOC 容器的模组
    /// </summary>
    public class FxAutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FxCacheDeleteInterceptor>();
            builder.RegisterType<UserManagerService>()
                .As<IUserManagerService>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(FxCacheDeleteInterceptor));
            base.Load(builder);
        }
    }
}
