using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreStudy.First.EFCore.Entity.EntityConfig
{
   public class OrgUnitConfig : IEntityTypeConfiguration<OrgUnit>
    {
        public void Configure(EntityTypeBuilder<OrgUnit> builder)
        {
            //命名规范  表 T_{object}s
            builder.ToTable("T_OrgUnit");
            builder.HasOne<OrgUnit>(c => c.Parent).WithMany(a => a.Children);
            builder.Property(o => o.Name).IsUnicode().IsRequired().HasMaxLength(50);

        }
    }
}
