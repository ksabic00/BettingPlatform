using BettingPlatform.Application.Common.Interfaces;
using BettingPlatform.Application.Wallets.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BettingPlatform.Application.Wallets.Queries.GetWallet;

public sealed class GetWalletHandler : IRequestHandler<GetWalletQuery, WalletDto>
{
    private readonly IAppDbContext _db;
    public GetWalletHandler(IAppDbContext db) => _db = db;

    public async Task<WalletDto> Handle(GetWalletQuery q, CancellationToken ct)
    {
        var w = await _db.Wallets.FirstOrDefaultAsync(x => x.PlayerId == q.PlayerId, ct)
                ?? throw new KeyNotFoundException("Wallet not found.");

        var tx = await _db.WalletTransactions
            .Where(t => t.WalletId == w.Id)
            .OrderByDescending(t => t.OccurredAtUtc)
            .Take(20)
            .Select(t => new WalletTransactionDto
            {
                OccurredAtUtc = t.OccurredAtUtc,
                Type = t.Type,
                Amount = t.Amount,
                ReferenceId = t.ReferenceId
            })
            .ToListAsync(ct);

        return new WalletDto
        {
            PlayerId = q.PlayerId,
            Balance = w.Balance,
            LastTransactions = tx
        };
    }
}
