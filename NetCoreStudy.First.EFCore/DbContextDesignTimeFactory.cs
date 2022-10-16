using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreStudy.First.EFCore
{
    public class DbContextDesignTimeFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<TestDbContext> builder = new DbContextOptionsBuilder<TestDbContext>();
            string connStr = "Data Source=.;Initial Catalog=Demo1;User ID=sa;Password=his";
            builder.UseSqlServer(connStr);
            return new TestDbContext(builder.Options);
        }
    }
}
