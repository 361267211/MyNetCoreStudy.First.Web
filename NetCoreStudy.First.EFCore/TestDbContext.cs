using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetCoreStudy.First.EFCore.Entity;
using NetCoreStudy.First.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreStudy.First.EFCore
{
    public class TestDbContext : IdentityDbContext<MyUser,MyRole,long>
    {
        private IMediator _mediator;
        public TestDbContext(DbContextOptions<TestDbContext> opt) : base(opt)
        {
        }

       /* public TestDbContext(DbContextOptions<TestDbContext> opt, IMediator mediator) : base(opt)
        {
            _mediator = mediator;
        }*/
        // private static ILoggerFactory _loggerFactory = LoggerFactory.Create(b => b.AddConsole());
        public DbSet<Book> Books { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<OrgUnit> OrgUnits { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Merchan> Marchans { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
           // string connectionString = "Data Source=.;Initial Catalog=Demo1;User ID=sa;Password=his";
          //  optionsBuilder.UseSqlServer(connectionString);
            //  optionsBuilder.UseLoggerFactory(_loggerFactory);
            //输出的信息更详细，但可读性很差
            optionsBuilder.LogTo(msg =>
            {
/*                //根据包含字符串暴力过滤掉非 SQL的log
                if (!msg.Contains(".CommandExecuted"))
                {
                    return;
                }
                Console.WriteLine(msg);*/
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //从当前程序集加载所有表
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = this.ChangeTracker.Entries<IDomainEvents>()
                .Where(e => e.Entity.GetDomainEvents().Any());
            var domainEvents = domainEntities.SelectMany(e => e.Entity.GetDomainEvents()).ToList();
            domainEntities.ToList().ForEach(e => e.Entity.ClearDomainEvents());

            foreach (var item in domainEvents)
            {
              await  _mediator.Publish(item);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }


}
