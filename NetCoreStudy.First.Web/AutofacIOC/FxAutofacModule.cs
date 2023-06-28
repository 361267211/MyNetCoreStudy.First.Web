using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using NetCoreStudy.First.Domain.FxService;
using NetCoreStudy.First.Web.FxRepository;

namespace NetCoreStudy.First.Web.AutofacIOC
{
    /// <summary>
    /// 配置Autofac IOC 容器的模组
    /// </summary>
    public class FxAutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //注册删除缓存装饰器
            builder.RegisterType<FxCachingInterceptor>();
            builder.RegisterType<UserManagerRepository>()
                .As<IUserManagerRepository>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(FxCachingInterceptor));



            base.Load(builder);
        }
    }
}
