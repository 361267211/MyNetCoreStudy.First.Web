using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCoreStudy.First.Domain.Entity.Fond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.EFCore.Configs
{
    internal class FondEventConfig : IEntityTypeConfiguration<FxFondEvent>
    {
        public void Configure(EntityTypeBuilder<FxFondEvent> builder)
        {
          

            builder.HasMany(e => e.Fonds)
                .WithOne(e => e.FondEvent)
                .HasForeignKey(e => e.FxFondEventId)
                .IsRequired();
        }
    }
}
