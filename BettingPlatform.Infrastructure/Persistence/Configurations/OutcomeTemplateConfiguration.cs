using BettingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingPlatform.Infrastructure.Persistence.Configurations;

public class OutcomeTemplateConfiguration : IEntityTypeConfiguration<OutcomeTemplate>
{
    public void Configure(EntityTypeBuilder<OutcomeTemplate> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
        b.Property(x => x.Code).HasMaxLength(10).IsRequired();
        b.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();

        b.HasIndex(x => x.Code).IsUnique();
    }
}
