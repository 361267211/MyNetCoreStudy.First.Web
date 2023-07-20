using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using NetCoreStudy.First.Web.FxRepository.FxFondRep;
using NetCoreStudy.First.Web.FxRepository.FxUser;
using NetCoreStudy.First.Web.FxService.FxFondService;
using StackExchange.Redis;

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
            builder.RegisterType<FxCachingChangeInterceptor>();
            builder.RegisterType<UserManagerService>()
                .As<IUserManagerService>();
            builder.RegisterType<UserManagerRepository>()
                .As<IUserManagerRepository>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(FxCachingInterceptor), typeof(FxCachingChangeInterceptor))
                ;


            builder.RegisterType<FondRepository>()
                .As<IFondRepository>();
            builder.RegisterType<FondService>()
                .As<IFondService>();

            builder.RegisterType<ContactRepository>()
                .As<IContactRepository>();
            builder.RegisterType<ContactService>()
                .As<IContactService>();

            builder.RegisterType<FondEventRepository>()
                .As<IFondEventRepository>();
            builder.RegisterType<FondEventService>()
              .As<IFondEventService>();


            base.Load(builder);
        }
    }
}
