using BettingPlatform.Application.Common.Interfaces;
using BettingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Player> Players => Set<Player>();
        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();

        public DbSet<Match> Matches => Set<Match>();
        public DbSet<MarketTemplate> MarketTemplates => Set<MarketTemplate>();
        public DbSet<OutcomeTemplate> OutcomeTemplates => Set<OutcomeTemplate>();
        public DbSet<MarketTemplateOutcome> MarketTemplateOutcomes => Set<MarketTemplateOutcome>();

        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<OfferOutcome> OfferOutcomes => Set<OfferOutcome>();

        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketSelection> TicketSelections => Set<TicketSelection>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
