using BettingPlatform.Application.Common.Interfaces;
using BettingPlatform.Application.Offers.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BettingPlatform.Application.Offers.Queries.ListOffersGrouped;

public sealed class ListOffersGroupedHandler
    : IRequestHandler<ListOffersGroupedQuery, IReadOnlyList<MatchOfferDto>>
{
    private readonly IAppDbContext _db;
    public ListOffersGroupedHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<MatchOfferDto>> Handle(ListOffersGroupedQuery q, CancellationToken ct)
    {
        var now = q.AsOfUtc ?? DateTime.UtcNow;

        var offers = await _db.Offers
            .Where(o => o.ValidFromUtc <= now && (o.ValidToUtc == null || o.ValidToUtc >= now))
            .Join(_db.Matches, o => o.MatchId, m => m.Id, (o, m) => new { o, m })
            .Join(_db.MarketTemplates, om => om.o.MarketTemplateId, mt => mt.Id, (om, mt) => new { om.o, om.m, mt })
            .AsNoTracking()
            .ToListAsync(ct);

        if (offers.Count == 0) return Array.Empty<MatchOfferDto>();

        var offerIds = offers.Select(x => x.o.Id).ToList();

        var outcomes = await _db.OfferOutcomes
            .Where(oo => offerIds.Contains(oo.OfferId))
            .Join(_db.OutcomeTemplates, oo => oo.OutcomeTemplateId, ot => ot.Id, (oo, ot) => new { oo, ot })
            .AsNoTracking()
            .ToListAsync(ct);

        var outcomesByOffer = outcomes
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

        var grouped = offers
            .GroupBy(x => x.o.MatchId)
            .Select(g =>
            {
                var any = g.First();
                return new MatchOfferDto
                {
                    MatchId = any.m.Id,
                    HomeTeam = any.m.HomeTeam,
                    AwayTeam = any.m.AwayTeam,
                    StartsAtUtc = any.m.StartsAtUtc,
                    Markets = g.Select(x => new MarketOfferDto
                    {
                        OfferId = x.o.Id,
                        MarketCode = x.mt.Code,
                        Category = x.o.Category,
                        Outcomes = outcomesByOffer.TryGetValue(x.o.Id, out var list) ? list : Array.Empty<OfferOutcomeDto>()
                    })
                    .OrderBy(m => m.MarketCode)
                    .ToList()
                };
            })
            .OrderBy(x => x.StartsAtUtc)
            .ToList();

        return grouped;
    }
}
