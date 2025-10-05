using BettingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BettingPlatform.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Player> Players { get; }
    DbSet<Wallet> Wallets { get; }
    DbSet<WalletTransaction> WalletTransactions { get; }
    DbSet<Match> Matches { get; }
    DbSet<Offer> Offers { get; }
    DbSet<OfferOutcome> OfferOutcomes { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<TicketSelection> TicketSelections { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
