using BettingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingPlatform.Infrastructure.Persistence.Configurations;

public class TicketSelectionConfiguration : IEntityTypeConfiguration<TicketSelection>
{
    public void Configure(EntityTypeBuilder<TicketSelection> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();

        b.Property(x => x.OddAtPlacement).HasPrecision(6, 2);

        b.HasIndex(x => new { x.TicketId, x.MatchId }).IsUnique();

        b.HasIndex(x => x.TicketId)
         .IsUnique()
         .HasDatabaseName("IX_TicketSelection_TopOnly")
         .HasFilter("[CategoryAtPlacement] = 1");

        b.HasOne<Ticket>().WithMany().HasForeignKey(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<Offer>().WithMany().HasForeignKey(x => x.OfferId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Match>().WithMany().HasForeignKey(x => x.MatchId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<OutcomeTemplate>().WithMany().HasForeignKey(x => x.OutcomeTemplateId).OnDelete(DeleteBehavior.Restrict);
    }
}
