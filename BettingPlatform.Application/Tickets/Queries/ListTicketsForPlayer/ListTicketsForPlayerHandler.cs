using BettingPlatform.Application.Common.Interfaces;
using BettingPlatform.Application.Tickets.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BettingPlatform.Application.Tickets.Queries.ListTicketsForPlayer;

public sealed class ListTicketsForPlayerHandler
    : IRequestHandler<ListTicketsForPlayerQuery, IReadOnlyList<TicketSummaryDto>>
{
    private readonly IAppDbContext _db;
    public ListTicketsForPlayerHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<TicketSummaryDto>> Handle(ListTicketsForPlayerQuery q, CancellationToken ct)
    {
        return await _db.Tickets
            .Where(t => t.PlayerId == q.PlayerId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .Select(t => new TicketSummaryDto
            {
                TicketId = t.Id,
                CreatedAtUtc = t.CreatedAtUtc,
                StakeGross = t.StakeGross,
                StakeNet = t.StakeNet,
                CombinedOdds = t.CombinedOdds,
                PotentialPayout = t.PotentialPayout,
                Status = t.Status.ToString(),
                SelectionCount = _db.TicketSelections.Count(s => s.TicketId == t.Id)
            })
            .ToListAsync(ct);
    }
}
