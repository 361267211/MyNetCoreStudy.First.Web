using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.Domain.Entity;
using System.Reflection;

namespace NetCoreStudy.First.EFCore
{
    public class UserDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserLoginHistory> UserLoginHistories { get; set; }
        public UserDbContext(DbContextOptions<UserDbContext> opt) : base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());




        }

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

    }
}
