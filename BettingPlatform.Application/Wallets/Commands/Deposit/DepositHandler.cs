using BettingPlatform.Application.Common.Interfaces;
using BettingPlatform.Application.Wallets.Dtos;
using BettingPlatform.Domain.Entities;
using BettingPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BettingPlatform.Application.Wallets.Commands.Deposit;

public sealed class DepositHandler : IRequestHandler<DepositCommand, WalletDto>
{
    private readonly IAppDbContext _db;
    public DepositHandler(IAppDbContext db) => _db = db;

    public async Task<WalletDto> Handle(DepositCommand cmd, CancellationToken ct)
    {
        var w = await _db.Wallets.FirstOrDefaultAsync(x => x.PlayerId == cmd.PlayerId, ct)
                ?? throw new KeyNotFoundException("Wallet not found.");

        _db.WalletTransactions.Add(new WalletTransaction
        {
            WalletId = w.Id,
            OccurredAtUtc = DateTime.UtcNow,
            Type = WalletTransactionType.Deposit,
            Amount = cmd.Amount,
            Note = "User deposit"
        });

        w.Balance += cmd.Amount;
        await _db.SaveChangesAsync(ct);

        return new WalletDto
        {
            PlayerId = cmd.PlayerId,
            Balance = w.Balance,
            LastTransactions = Array.Empty<WalletTransactionDto>()
        };
    }
}
