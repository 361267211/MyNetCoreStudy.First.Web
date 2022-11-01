using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCoreStudy.First.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStudy.First.EFCore.Configs
{
    public class UserAccessFailConfig : IEntityTypeConfiguration<UserAccessFail>
    {
        public void Configure(EntityTypeBuilder<UserAccessFail> builder)
        {
            builder.ToTable("T_UserAccessFails");
            builder.Property("lockOut");
        }
    }
}
