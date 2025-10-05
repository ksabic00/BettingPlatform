using BettingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingPlatform.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();

        b.Property(x => x.StakeGross).HasPrecision(12, 2);
        b.Property(x => x.FeeAmount).HasPrecision(12, 2);
        b.Property(x => x.StakeNet).HasPrecision(12, 2);
        b.Property(x => x.CombinedOdds).HasPrecision(10, 2);
        b.Property(x => x.PotentialPayout).HasPrecision(14, 2);
        b.Property(x => x.FeePercent).HasPrecision(5, 4);   
        b.Property(x => x.PayoutAmount).HasPrecision(14, 2);
        b.Property(x => x.CreatedAtUtc).IsRequired();

        b.HasOne<Player>()
         .WithMany()
         .HasForeignKey(x => x.PlayerId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
