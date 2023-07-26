using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FxCode.FxDatabaseAccessor
{
    public static class DatabaseAccessorServiceCollectionExtensions
    {
        /// <summary>
        /// 注入工作单元
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <param name="migrationAssemblyName"></param>
        /// <returns></returns>
        public static IServiceCollection AddDatabaseAccessor(this IServiceCollection services, Action<IServiceCollection> configure = null, string migrationAssemblyName = default)
        {

            // 注册自动 SaveChanges
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add<AutoSaveChangesFilter>();
            });
            //注入工作单元
            services.AddUnitOfWork<FxFondUnitOfWork>();
            return services;
        }
    }
}
