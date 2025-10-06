using BettingPlatform.Application.Wallets.Dtos;
using MediatR;

namespace BettingPlatform.Application.Wallets.Queries.GetWallet;

public sealed class GetWalletQuery : IRequest<WalletDto>
{
    public Guid PlayerId { get; }
    public GetWalletQuery(Guid playerId) => PlayerId = playerId;
}
