using BettingPlatform.Application.Common.Interfaces;
using BettingPlatform.Application.Players.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BettingPlatform.Application.Players.Queries.ListPlayers;
public sealed class ListPlayersHandler : IRequestHandler<ListPlayersQuery, IReadOnlyList<PlayerSummaryDto>>
{
    private readonly IAppDbContext _db;
    public ListPlayersHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<PlayerSummaryDto>> Handle(ListPlayersQuery q, CancellationToken ct)
        => await _db.Players
            .OrderBy(p => p.DisplayName)
            .Select(p => new PlayerSummaryDto { Id = p.Id, DisplayName = p.DisplayName })
            .ToListAsync(ct);
}
