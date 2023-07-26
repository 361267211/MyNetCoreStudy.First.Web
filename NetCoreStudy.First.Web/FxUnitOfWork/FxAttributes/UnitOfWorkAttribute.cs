using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace FxCode.FxDatabaseAccessor
{
    /// <summary>
    /// 工作单元特性，只在controller的method中生效，
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitOfWorkAttribute : Attribute, IAsyncActionFilter, IAsyncPageFilter, IOrderedFilter
    {
        /// <summary>
        /// 是否使用分布式环境事务
        /// </summary>
        public bool UseAmbientTransaction { get; set; } = false;

        /// <summary>
        ///  MiniProfiler 分类名
        /// </summary>
        private const string MiniProfilerCategory = "unitOfWork";

        /// <summary>
        /// 分布式环境事务范围
        /// </summary>
        /// <remarks><see cref="UseAmbientTransaction"/> 为 true 有效</remarks>
        public TransactionScopeOption TransactionScope { get; set; } = TransactionScopeOption.Required;

        /// <summary>
        /// 分布式环境事务隔离级别
        /// </summary>
        /// <remarks><see cref="UseAmbientTransaction"/> 为 true 有效</remarks>
        public IsolationLevel TransactionIsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        /// <summary>
        /// 分布式环境事务超时时间
        /// </summary>
        /// <remarks>单位秒</remarks>
        public int TransactionTimeout { get; set; } = 0;

        /// <summary>
        /// 过滤器排序
        /// </summary>
        private const int FilterOrder = 9999;

        /// <summary>
        /// 支持分布式环境事务异步流
        /// </summary>
        /// <remarks><see cref="UseAmbientTransaction"/> 为 true 有效</remarks>
        public TransactionScopeAsyncFlowOption TransactionScopeAsyncFlow { get; set; } = TransactionScopeAsyncFlowOption.Enabled;

        public int Order => FilterOrder;

        private readonly ICapPublisher _capBus;


        /// <summary>
        /// 拦截请求
        /// </summary>
        /// <param name="context">动作方法上下文</param>
        /// <param name="next">中间件委托</param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 获取动作方法描述器
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var method = actionDescriptor.MethodInfo;

            // 创建分布式环境事务
            (var transactionScope, var logger) = CreateTransactionScope(context);

            try
            {
                // 打印工作单元开始消息
              //  if (UseAmbientTransaction) //App.PrintToMiniProfiler(MiniProfilerCategory, "Beginning (Ambient)");

                   
                // 开始事务
                BeginTransaction(context, method, out var _unitOfWork, out var unitOfWorkAttribute);

                // 获取执行 Action 结果
                var resultContext = await next();

                // 提交事务
                CommitTransaction(context, _unitOfWork, unitOfWorkAttribute, resultContext);

                // 提交分布式环境事务
                if (resultContext.Exception == null)
                {
                    transactionScope?.Complete();

                    // 打印事务提交消息
                   // if (UseAmbientTransaction) //App.PrintToMiniProfiler(MiniProfilerCategory, "Completed (Ambient)");
                }
                else
                {
                    // 打印事务回滚消息
                  //  if (UseAmbientTransaction) //App.PrintToMiniProfiler(MiniProfilerCategory, "Rollback (Ambient)", isError: true);

                    logger.LogError(resultContext.Exception, "Transaction Failed.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Transaction Failed.");

                // 打印事务回滚消息
               // if (UseAmbientTransaction) //App.PrintToMiniProfiler(MiniProfilerCategory, "Rollback (Ambient)", isError: true);

                throw;
            }
            finally
            {
                transactionScope?.Dispose();
            }
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="method"></param>
        /// <param name="_unitOfWork"></param>
        /// <param name="unitOfWorkAttribute"></param>
        private static void BeginTransaction(FilterContext context, MethodInfo method, out IUnitOfWork _unitOfWork, out UnitOfWorkAttribute unitOfWorkAttribute)
        {
            // 解析工作单元服务
            _unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();

            // 获取工作单元特性
            unitOfWorkAttribute = method.GetCustomAttribute<UnitOfWorkAttribute>();

            // 调用开启事务方法
            _unitOfWork.BeginTransaction(context, unitOfWorkAttribute);

            // 打印工作单元开始消息
            ////if (!unitOfWorkAttribute.UseAmbientTransaction)  //App.PrintToMiniProfiler(MiniProfilerCategory, "Beginning");
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="_unitOfWork"></param>
        /// <param name="unitOfWorkAttribute"></param>
        /// <param name="resultContext"></param>
        private static void CommitTransaction(FilterContext context, IUnitOfWork _unitOfWork, UnitOfWorkAttribute unitOfWorkAttribute, FilterContext resultContext)
        {
            // 获取动态结果上下文
            dynamic dynamicResultContext = resultContext;

            if (dynamicResultContext.Exception == null)
            {
                // 调用提交事务方法
                _unitOfWork.CommitTransaction(resultContext, unitOfWorkAttribute);
            }
            else
            {
                // 调用回滚事务方法
                _unitOfWork.RollbackTransaction(resultContext, unitOfWorkAttribute);
            }

            // 调用执行完毕方法
            _unitOfWork.OnCompleted(context, resultContext);

            // 打印工作单元结束消息
            //if (!unitOfWorkAttribute.UseAmbientTransaction) //App.PrintToMiniProfiler(MiniProfilerCategory, "Ending");
        }

        /// <summary>
        /// 创建分布式环境事务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private (TransactionScope, ILogger) CreateTransactionScope(FilterContext context)
        {
            // 是否启用分布式环境事务
            var transactionScope = UseAmbientTransaction
                 ? new TransactionScope(TransactionScope,
                new TransactionOptions { IsolationLevel = TransactionIsolationLevel, Timeout = TransactionTimeout > 0 ? TimeSpan.FromSeconds(TransactionTimeout) : default }
                , TransactionScopeAsyncFlow)
                 : default;

            // 创建日志记录器
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                 .CreateLogger(GetType().FullName);

            return (transactionScope, logger);
        }



        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
              return Task.CompletedTask;

        }

        /// <summary>
        /// 拦截请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            // 获取动作方法描述器
            var method = context.HandlerMethod?.MethodInfo;
            // 处理 Blazor Server
            if (method == null)
            {
                _ = await next.Invoke();
                return;
            }

            // 创建分布式环境事务
            (var transactionScope, var logger) = CreateTransactionScope(context);

            try
            {
                // 打印工作单元开始消息
                //if (UseAmbientTransaction) App.PrintToMiniProfiler(MiniProfilerCategory, "Beginning (Ambient)");

                // 开始事务
                BeginTransaction(context, method, out var _unitOfWork, out var unitOfWorkAttribute);

                // 获取执行 Action 结果
                var resultContext = await next.Invoke();

                // 提交事务
                CommitTransaction(context, _unitOfWork, unitOfWorkAttribute, resultContext);

                // 提交分布式环境事务
                if (resultContext.Exception == null)
                {
                    transactionScope?.Complete();

                    // 打印事务提交消息
                    //if (UseAmbientTransaction) App.PrintToMiniProfiler(MiniProfilerCategory, "Completed (Ambient)");
                }
                else
                {
                    // 打印事务回滚消息
                    //if (UseAmbientTransaction) App.PrintToMiniProfiler(MiniProfilerCategory, "Rollback (Ambient)", isError: true);

                    logger.LogError(resultContext.Exception, "Transaction Failed.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Transaction Failed.");

                // 打印事务回滚消息
                //if (UseAmbientTransaction) App.PrintToMiniProfiler(MiniProfilerCategory, "Rollback (Ambient)", isError: true);

                throw;
            }
            finally
            {
                transactionScope?.Dispose();
            }
        }
    }
}
