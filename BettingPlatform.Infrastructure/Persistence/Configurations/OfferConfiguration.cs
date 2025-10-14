using BettingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingPlatform.Infrastructure.Persistence.Configurations;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();

        b.HasOne<Match>()
         .WithMany()
         .HasForeignKey(x => x.MatchId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne<MarketTemplate>()
         .WithMany()
         .HasForeignKey(x => x.MarketTemplateId)
         .OnDelete(DeleteBehavior.Restrict);

        b.Property(x => x.ValidFromUtc).IsRequired();
        b.HasIndex(x => new { x.MatchId, x.MarketTemplateId }).IsUnique();

    }
}
