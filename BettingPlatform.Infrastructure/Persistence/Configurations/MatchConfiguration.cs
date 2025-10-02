using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BettingPlatform.Domain.Entities;

namespace BettingPlatform.Infrastructure.Persistence.Configurations
{
    public class MatchConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
             .HasDefaultValueSql("NEWSEQUENTIALID()")
             .ValueGeneratedOnAdd();

            b.Property(x => x.HomeTeam).HasMaxLength(100).IsRequired();
            b.Property(x => x.AwayTeam).HasMaxLength(100).IsRequired();
            b.Property(x => x.Sport).HasMaxLength(50).HasDefaultValue("football");
            b.Property(x => x.Status).HasMaxLength(50);
        }
    }
}
