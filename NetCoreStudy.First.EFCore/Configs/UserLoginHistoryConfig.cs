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
    public class UserLoginHistoryConfig : IEntityTypeConfiguration<UserLoginHistory>
    {
        public void Configure(EntityTypeBuilder<UserLoginHistory> builder)
        {
            builder.ToTable("T_UserLoginHistories");
            builder.OwnsOne(e => e.PhoneNumber, pn =>
            {
                pn.Property(s => s.Number).HasMaxLength(20).IsUnicode(false);
            });

        }
    }
}
