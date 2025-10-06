using BettingPlatform.Application.Common.Interfaces;
using BettingPlatform.Domain.Entities;
using MediatR;

namespace BettingPlatform.Application.Players.Commands.CreatePlayer;

public sealed class CreatePlayerHandler : IRequestHandler<CreatePlayerCommand, Guid>
{
    private readonly IAppDbContext _db;
    public CreatePlayerHandler(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreatePlayerCommand req, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var player = new Player { DisplayName = req.DisplayName };
        _db.Players.Add(player);
        await _db.SaveChangesAsync(ct);            

        _db.Wallets.Add(new Wallet { PlayerId = player.Id, Balance = 0m });
        await _db.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);
        return player.Id;
    }
}
