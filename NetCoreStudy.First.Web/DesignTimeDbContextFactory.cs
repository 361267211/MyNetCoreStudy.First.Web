using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.BasicModel;

namespace NetCoreStudy.First.Web
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(GlobalConfigOption.DbContext.DbConnection);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }

}
