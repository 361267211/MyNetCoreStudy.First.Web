using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreStudy.First.EFCore.Entity.EntityConfig
{
   public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("T_Students");
            builder.HasMany<Teacher>(s=>s.Teachers).WithMany(t => t.Students).UsingEntity(j=>j.ToTable("T_Students_Teachers"));
        }
    }
}

