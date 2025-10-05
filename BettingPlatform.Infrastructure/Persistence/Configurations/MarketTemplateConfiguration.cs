using BettingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingPlatform.Infrastructure.Persistence.Configurations;

public class MarketTemplateConfiguration : IEntityTypeConfiguration<MarketTemplate>
{
    public void Configure(EntityTypeBuilder<MarketTemplate> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
        b.Property(x => x.Code).HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();

        b.HasIndex(x => x.Code).IsUnique(); 
    }
}
