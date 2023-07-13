using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCoreStudy.First.Domain.Entity;
using NetCoreStudy.First.Domain.Entity.Fond;

namespace NetCoreStudy.First.EFCore.Configs
{
    internal class FondConfig : IEntityTypeConfiguration<FxFond>
    {
        public void Configure(EntityTypeBuilder<FxFond> builder)
        {
    
        }
    }
}