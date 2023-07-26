using System;
using System.Transactions;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using NetCoreStudy.First.EFCore;

namespace FxCode.FxDatabaseAccessor
{
    public class FxFondUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly FondDbContext _fondDbContext;

        private readonly ICapPublisher _capBus;

        public FxFondUnitOfWork(FondDbContext fondDbContext)
        {
            _fondDbContext = fondDbContext;
        }

        /// <summary>
        /// 数据库上下文事务
        /// </summary>
        public IDbContextTransaction DbContextTransaction { get; private set; }

        public void BeginTransaction(FilterContext context, UnitOfWorkAttribute unitOfWork)
        {
            // 判断是否启用了分布式环境事务，如果是，则跳过
            if (Transaction.Current != null) return;
            this.DbContextTransaction = _fondDbContext.Database.BeginTransaction();

        }

        public void CommitTransaction(FilterContext resultContext, UnitOfWorkAttribute unitOfWork)
        {
            if (Transaction.Current != null) return;

            this.DbContextTransaction.Commit();
        }

        public void OnCompleted(FilterContext context, FilterContext resultContext)
        {
            Console.WriteLine("事务完成");
        }

        public void RollbackTransaction(FilterContext resultContext, UnitOfWorkAttribute unitOfWork)
        {
            // 判断是否启用了分布式环境事务，如果是，则跳过
            if (Transaction.Current != null) return;
            this.DbContextTransaction?.Rollback();
        }
    }
}
