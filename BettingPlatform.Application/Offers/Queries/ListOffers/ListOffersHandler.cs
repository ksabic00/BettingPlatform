using BettingPlatform.Application.Common.Interfaces;
using BettingPlatform.Application.Offers.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BettingPlatform.Application.Offers.Queries.ListOffers;

public sealed class ListOffersHandler : IRequestHandler<ListOffersQuery, IReadOnlyList<OfferDto>>
{
    private readonly IAppDbContext _db;
    public ListOffersHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<OfferDto>> Handle(ListOffersQuery q, CancellationToken ct)
    {
        var now = q.AsOfUtc ?? DateTime.UtcNow;

        var offers = await _db.Offers
            .Where(o => o.ValidFromUtc <= now && (o.ValidToUtc == null || o.ValidToUtc >= now))
            .Join(_db.Matches, o => o.MatchId, m => m.Id, (o, m) => new { o, m })
            .Join(_db.MarketTemplates, om => om.o.MarketTemplateId, mt => mt.Id, (om, mt) => new { om.o, om.m, mt })
            .AsNoTracking()
            .ToListAsync(ct);

        if (!offers.Any()) return Array.Empty<OfferDto>();  

        var offerIds = offers.Select(x => x.o.Id).ToList();

        var outcomes = await _db.OfferOutcomes
            .Where(oo => offerIds.Contains(oo.OfferId))
            .Join(_db.OutcomeTemplates, oo => oo.OutcomeTemplateId, ot => ot.Id, (oo, ot) => new { oo, ot })
            .AsNoTracking()
            .ToListAsync(ct);

        var outcomeLookup = outcomes
            .GroupBy(x => x.oo.OfferId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<OfferOutcomeDto>)g.Select(x => new OfferOutcomeDto
                {
                    OutcomeTemplateId = x.ot.Id,
                    OutcomeCode = x.ot.Code,
                    Odds = x.oo.Odds,
                    IsEnabled = x.oo.IsEnabled
                }).ToList()
            );

        var result = offers
            .Select(x => new OfferDto
            {
                OfferId = x.o.Id,
                MatchId = x.o.MatchId,
                HomeTeam = x.m.HomeTeam,
                AwayTeam = x.m.AwayTeam,
                StartsAtUtc = x.m.StartsAtUtc,
                MarketCode = x.mt.Code,
                Category = x.o.Category,
                Outcomes = outcomeLookup.TryGetValue(x.o.Id, out var list) ? list : Array.Empty<OfferOutcomeDto>()
            })
            .OrderBy(d => d.StartsAtUtc)
            .ToList();

        return result;
    }
}
