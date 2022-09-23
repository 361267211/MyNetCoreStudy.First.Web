using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreStudy.First.EFCore.Entity.EntityConfig
{
    class ArticleConfig : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            //命名规范  表 T_{object}s
            builder.ToTable("T_Article");
            builder.HasQueryFilter(e=>e.IsDeleted==false);
            builder.Property(b => b.Content).IsRequired().IsUnicode();
            builder.Property(b => b.Title).IsRequired().IsUnicode().HasMaxLength(255);
            
        }
    }
}
