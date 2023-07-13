using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.EFCore.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.EFCore
{
    public class FondDbContext : DbContext
    {
        public DbSet<FxFond> Fonds { get; set; }
        public DbSet<FxFondEvent> FondEvents { get; set; }
        public DbSet<FxContact> FondContacts { get; set; }
        public FondDbContext(DbContextOptions<FondDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FondEventConfig());
            //modelBuilder.ApplyConfiguration(new FondConfig());

            base.OnModelCreating(modelBuilder);


        }
    }
}
