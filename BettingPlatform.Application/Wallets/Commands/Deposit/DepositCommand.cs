using BettingPlatform.Application.Wallets.Dtos;
using MediatR;

namespace BettingPlatform.Application.Wallets.Commands.Deposit;

public sealed class DepositCommand : IRequest<WalletDto>
{
    public Guid PlayerId { get; init; }
    public decimal Amount { get; init; }
}
