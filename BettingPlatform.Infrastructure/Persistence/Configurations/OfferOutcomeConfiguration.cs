using BettingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingPlatform.Infrastructure.Persistence.Configurations;

public class OfferOutcomeConfiguration : IEntityTypeConfiguration<OfferOutcome>
{
    public void Configure(EntityTypeBuilder<OfferOutcome> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();

        b.Property(x => x.Odds).HasPrecision(6, 2);

        b.HasOne<Offer>()
         .WithMany()
         .HasForeignKey(x => x.OfferId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<OutcomeTemplate>()
         .WithMany()
         .HasForeignKey(x => x.OutcomeTemplateId)
         .OnDelete(DeleteBehavior.Restrict);

        b.ToTable(t => t.HasCheckConstraint("CK_OfferOutcome_Odds_Min", "[Odds] >= 1.00"));
        b.HasIndex(x => new { x.OfferId, x.OutcomeTemplateId }).IsUnique();

    }
}
