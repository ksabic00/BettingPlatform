using BettingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingPlatform.Infrastructure.Persistence.Configurations;

public class MarketTemplateOutcomeConfiguration : IEntityTypeConfiguration<MarketTemplateOutcome>
{
    public void Configure(EntityTypeBuilder<MarketTemplateOutcome> b)
    {
        b.HasKey(x => new { x.MarketTemplateId, x.OutcomeTemplateId });

        b.HasOne<MarketTemplate>()
         .WithMany()
         .HasForeignKey(x => x.MarketTemplateId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<OutcomeTemplate>()
         .WithMany()
         .HasForeignKey(x => x.OutcomeTemplateId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
