using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreStudy.First.EFCore.Entity.EntityConfig
{
    class CommentConfig : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            //命名规范  表 T_{object}s
            builder.ToTable("T_Comment");
            builder.HasOne<Article>(c => c.Article).WithMany(a => a.Comments).IsRequired();
            builder.Property(c => c.Message).IsRequired().IsUnicode();
        }
    }
}
