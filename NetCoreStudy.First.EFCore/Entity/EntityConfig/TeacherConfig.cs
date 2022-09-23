using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreStudy.First.EFCore.Entity.EntityConfig
{
    class TeacherConfig : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.ToTable("T_Teachers");
            builder.HasMany<Student>(t=>t.Students).WithMany(s => s.Teachers);
        }
    }
}
