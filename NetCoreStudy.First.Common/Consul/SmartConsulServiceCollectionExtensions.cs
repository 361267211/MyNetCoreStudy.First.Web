/*********************************************************
* 名    称：SmartConsulServiceCollectionExtension.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210826
* 描    述：Consul注册扩展，随机选取配置的Consul端点的一半地址，向其进行服务注册，实现高可用
* 更新历史：
*
* *******************************************************/
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace NetCoreStudy.Core.Consul
{
    /// <summary>
    /// Consul服务注册扩展方法
    /// </summary>
    public static class SmartConsulServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Consul服务
        /// </summary>
        /// <param name="service">ServiceCollection</param>
        /// <param name="options">配置信息</param>
        /// <returns></returns>
        public static SmartConsulBuilder AddConsul(this IServiceCollection service, SmartConsulOptions options)
        {
            service.Configure(new Action<SmartConsulOptions>(op =>
            {
                op.Address = options.Address;
                op.Token = options.Token;
            }));
            var provider = service.BuildServiceProvider();
            var smartConsulOptions = provider.GetRequiredService<IOptions<SmartConsulOptions>>();
            return new SmartConsulBuilder(smartConsulOptions.Value.Address);
        }

        /// <summary>
        /// 添加Consul服务
        /// </summary>
        /// <param name="service">ServiceCollection</param>
        /// <param name="url">Consul Agent连接地址</param>
        /// <returns></returns>
        public static SmartConsulBuilder AddConsul(this IServiceCollection service, string url)
        {
            service.Configure(new Action<SmartConsulOptions>(op => op.Address = url));
            var provider = service.BuildServiceProvider();
            var smartConsulOptions = provider.GetRequiredService<IOptions<SmartConsulOptions>>();
            return new SmartConsulBuilder(smartConsulOptions.Value.Address);
        }
    }
}
