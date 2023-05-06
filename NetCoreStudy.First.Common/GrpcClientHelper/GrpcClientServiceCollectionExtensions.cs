/*********************************************************
* 名    称：GrpcClientServiceCollectionExtensions.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210827
* 描    述：Grpc客户端工厂扩展，构建缓存用于帮助重用信道
* 更新历史：
*
* *******************************************************/
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreStudy.Core.GrpcClientHelper
{
    /// <summary>
    /// Grpc客户端工厂扩展方法
    /// </summary>
    public static class GrpcClientServiceCollectionExtensions
    {
        /// <summary>
        /// 添加客户端工厂服务
        /// 注册SmartGrpcClientFactory单例,GrpcClientPool单例,GrpcChannelPool单例,以及IGrpcTargetAddressResolver默认实现
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddSmartGrpcClientFactory(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(SmartGrpcClientFactory<>));
            serviceCollection.AddSingleton(typeof(GrpcClientPool<>));
            serviceCollection.AddSingleton<GrpcChannelPool>();
            serviceCollection.AddScoped<IGrpcTargetAddressResolver, GrpcTargetAddressResolverDefault>();
            return serviceCollection;
        }
    }
}
