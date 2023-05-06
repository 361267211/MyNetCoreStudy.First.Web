using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace NetCoreStudy.First.Web.IdentityService4
{
    public class SeedData
    {
        public static void EnsureSeedData(string conncetionString)
        {
            var migtationAssembly = typeof(SeedData).GetTypeInfo().Assembly.GetName().Name;
            var service = new ServiceCollection();
            service.AddConfigurationDbContext(options =>
            {
                options.ConfigureDbContext = db => db.UseNpgsql(conncetionString,
                    sql => sql.MigrationsAssembly(migtationAssembly));
            });
            service.AddOperationalDbContext(options =>
            {
                options.ConfigureDbContext = db => db.UseNpgsql(conncetionString,
                    sql => sql.MigrationsAssembly(migtationAssembly));
            });

            var serviceProvider = service.BuildServiceProvider();

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                context.Database.Migrate();
                EnsureSeedData(context);
            }
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients)
                    context.Clients.Add(client.ToEntity());
                context.SaveChanges();
            }
   

            if (!context.IdentityResources.Any())
            {
                foreach (var id in Config.IdentityResources)
                    context.IdentityResources.Add(id.ToEntity());
                context.SaveChanges();
            }
        }
    }

}
