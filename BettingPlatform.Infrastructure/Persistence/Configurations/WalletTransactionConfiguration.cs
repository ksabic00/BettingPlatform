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
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
             .HasDefaultValueSql("NEWSEQUENTIALID()")
             .ValueGeneratedOnAdd();

            b.Property(x => x.Amount).HasPrecision(12, 2);

            b.HasOne<Wallet>()
             .WithMany()
             .HasForeignKey(x => x.WalletId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
