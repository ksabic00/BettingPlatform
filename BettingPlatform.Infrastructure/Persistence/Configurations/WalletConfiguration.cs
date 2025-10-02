using BettingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Infrastructure.Persistence.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
             .HasDefaultValueSql("NEWSEQUENTIALID()")
             .ValueGeneratedOnAdd();

            b.Property(x => x.Balance).HasPrecision(12, 2);
            b.Property(x => x.RowVersion).IsRowVersion();

            b.HasOne<Player>()
             .WithMany()
             .HasForeignKey(x => x.PlayerId)
             .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
