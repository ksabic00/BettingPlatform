using BettingPlatform.Domain.Entities;
using BettingPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BettingPlatform.Infrastructure.Persistence.Seed;

public static class InitialDataSeeder
{
    public static async Task SeedAsync(AppDbContext db, ILogger logger, CancellationToken ct = default)
    {
        if (await db.MarketTemplates.AnyAsync(ct))
        {
            logger.LogInformation("Initial seed already applied. Skipping.");
            return;
        }

        logger.LogInformation("Applying initial seed...");

        var m1x2 = new MarketTemplate { Code = "1X2", Name = "Match Result" };
        var mdc = new MarketTemplate { Code = "DoubleChance", Name = "Double Chance" };

        var o1 = new OutcomeTemplate { Code = "1", DisplayName = "Home win" };
        var oX = new OutcomeTemplate { Code = "X", DisplayName = "Draw" };
        var o2 = new OutcomeTemplate { Code = "2", DisplayName = "Away win" };
        var o1x = new OutcomeTemplate { Code = "1X", DisplayName = "Home or draw" };
        var ox2 = new OutcomeTemplate { Code = "X2", DisplayName = "Draw or away" };
        var o12 = new OutcomeTemplate { Code = "12", DisplayName = "Home or away" };

        db.MarketTemplates.AddRange(m1x2, mdc);
        db.OutcomeTemplates.AddRange(o1, oX, o2, o1x, ox2, o12);
        await db.SaveChangesAsync(ct);

        db.MarketTemplateOutcomes.AddRange(
            new MarketTemplateOutcome { MarketTemplateId = m1x2.Id, OutcomeTemplateId = o1.Id },
            new MarketTemplateOutcome { MarketTemplateId = m1x2.Id, OutcomeTemplateId = oX.Id },
            new MarketTemplateOutcome { MarketTemplateId = m1x2.Id, OutcomeTemplateId = o2.Id },
            new MarketTemplateOutcome { MarketTemplateId = mdc.Id, OutcomeTemplateId = o1x.Id },
            new MarketTemplateOutcome { MarketTemplateId = mdc.Id, OutcomeTemplateId = ox2.Id },
            new MarketTemplateOutcome { MarketTemplateId = mdc.Id, OutcomeTemplateId = o12.Id }
        );

        var matchA = new Match { HomeTeam = "Dinamo", AwayTeam = "Hajduk", StartsAtUtc = DateTime.UtcNow.AddDays(2), Sport = "football" };
        var matchB = new Match { HomeTeam = "Osijek", AwayTeam = "Rijeka", StartsAtUtc = DateTime.UtcNow.AddDays(3), Sport = "football" };
        db.Matches.AddRange(matchA, matchB);
        await db.SaveChangesAsync(ct);

        var offerA = new Offer { MatchId = matchA.Id, MarketTemplateId = m1x2.Id, Category = OfferCategory.Regular, ValidFromUtc = DateTime.UtcNow.AddHours(-1) };
        var offerB = new Offer { MatchId = matchB.Id, MarketTemplateId = m1x2.Id, Category = OfferCategory.Top, ValidFromUtc = DateTime.UtcNow.AddHours(-1) };
        db.Offers.AddRange(offerA, offerB);
        await db.SaveChangesAsync(ct);

        db.OfferOutcomes.AddRange(
            new OfferOutcome { OfferId = offerA.Id, OutcomeTemplateId = o1.Id, Odds = 1.85m, IsEnabled = true },
            new OfferOutcome { OfferId = offerA.Id, OutcomeTemplateId = oX.Id, Odds = 3.40m, IsEnabled = true },
            new OfferOutcome { OfferId = offerA.Id, OutcomeTemplateId = o2.Id, Odds = 4.10m, IsEnabled = true },

            new OfferOutcome { OfferId = offerB.Id, OutcomeTemplateId = o1.Id, Odds = 2.10m, IsEnabled = true },
            new OfferOutcome { OfferId = offerB.Id, OutcomeTemplateId = oX.Id, Odds = 3.20m, IsEnabled = true },
            new OfferOutcome { OfferId = offerB.Id, OutcomeTemplateId = o2.Id, Odds = 3.60m, IsEnabled = true }
        );

        await db.SaveChangesAsync(ct);

        logger.LogInformation("Initial seed applied.");
    }
}
