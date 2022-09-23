using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreStudy.First.EFCore
{
    class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            //命名规范  表 T_{object}s
            builder.ToTable("T_Books").HasIndex(b=>new {b.Title,b.Price });
            builder.Property(b => b.Price).HasMaxLength(10).IsRequired().IsConcurrencyToken();
            builder.Property(b => b.Title).HasMaxLength(20).IsRequired();
        }
    }
}
