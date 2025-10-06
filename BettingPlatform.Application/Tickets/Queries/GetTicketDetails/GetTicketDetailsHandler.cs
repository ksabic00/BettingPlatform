using BettingPlatform.Application.Common.Interfaces;
using BettingPlatform.Application.Tickets.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BettingPlatform.Application.Tickets.Queries.GetTicketDetails;

public sealed class GetTicketDetailsHandler : IRequestHandler<GetTicketDetailsQuery, TicketDetailDto>
{
    private readonly IAppDbContext _db;
    public GetTicketDetailsHandler(IAppDbContext db) => _db = db;

    public async Task<TicketDetailDto> Handle(GetTicketDetailsQuery q, CancellationToken ct)
    {
        var t = await _db.Tickets.FirstOrDefaultAsync(x => x.Id == q.TicketId, ct)
                ?? throw new KeyNotFoundException("Ticket not found.");

        var lines = await _db.TicketSelections
            .Where(s => s.TicketId == q.TicketId)
            .Join(_db.Matches, s => s.MatchId, m => m.Id, (s, m) => new { s, m })
            .Join(_db.OutcomeTemplates, sm => sm.s.OutcomeTemplateId, ot => ot.Id, (sm, ot) => new { sm.s, sm.m, ot })
            .Select(x => new TicketSelectionLineDto
            {
                HomeTeam = x.m.HomeTeam,
                AwayTeam = x.m.AwayTeam,
                OutcomeCode = x.ot.Code,
                OddAtPlacement = x.s.OddAtPlacement,
                CategoryAtPlacement = x.s.CategoryAtPlacement
            })
            .ToListAsync(ct);

        return new TicketDetailDto
        {
            TicketId = t.Id,
            CreatedAtUtc = t.CreatedAtUtc,
            StakeGross = t.StakeGross,
            StakeNet = t.StakeNet,
            CombinedOdds = t.CombinedOdds,
            PotentialPayout = t.PotentialPayout,
            Status = t.Status.ToString(),
            Selections = lines
        };
    }
}
