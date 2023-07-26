using Microsoft.Extensions.DependencyInjection;

namespace FxCode.FxDatabaseAccessor
{
    public static class UnitOfWorkServiceCollectionExtensions
    {

        /// <summary>
        /// 添加工作单元服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns>Mvc构建器</returns>
        public static IServiceCollection AddUnitOfWork<TUnitOfWork>(this IServiceCollection services)
            where TUnitOfWork : class, IUnitOfWork
        {
            // 注册工作单元服务
            services.AddTransient<IUnitOfWork, TUnitOfWork>();
            return services;
        }
    }
}
