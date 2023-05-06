/*********************************************************
* 名    称：RedisCacheServiceCollectionExtensions.cs
* 作    者：刘孟
* 联系方式：电话[13629774594],邮件[1450873843@qq.com]
* 创建时间：20210901
* 描    述：RedisCac he扩展方法，注入RedisCacheManager与配置项
* 更新历史：
*
* *******************************************************/
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.Core.Redis
{
    public static class RedisCacheServiceCollectionExtensions
    {
        /// <summary>
        /// 添加RedisCacheManager
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedisCacheManager(this IServiceCollection serviceCollection, Action<RedisCacheOptions> options)
        {
            serviceCollection.Configure(options);
            serviceCollection.AddSingleton<IRedisCacheManager, RedisCacheManager>();
            return serviceCollection;
        }

    }
}
