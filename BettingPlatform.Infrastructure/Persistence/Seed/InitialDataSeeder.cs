using BettingPlatform.Domain.Entities;
using BettingPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BettingPlatform.Infrastructure.Persistence.Seed;

public static class InitialDataSeeder
{
    public static async Task SeedAsync(AppDbContext db, ILogger logger, CancellationToken ct = default)
    {
        if (await db.Matches.AnyAsync(ct) || await db.Offers.AnyAsync(ct))
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
        await db.SaveChangesAsync(ct);

        var now = DateTime.UtcNow;
        var matches = Enumerable.Range(1, 10)
            .Select(i => new Match
            {
                HomeTeam = $"Team{i}A",
                AwayTeam = $"Team{i}B",
                StartsAtUtc = now.AddDays((i % 5) + 1),
                Sport = "football"
            })
            .ToList();

        db.Matches.AddRange(matches);
        await db.SaveChangesAsync(ct);

        var rnd = new Random(20251014);
        decimal Rand(decimal min, decimal max)
        {
            var v = (decimal)rnd.NextDouble() * (max - min) + min;
            return Math.Round(v, 2, MidpointRounding.AwayFromZero);
        }
        decimal Bump(decimal v, decimal factor = 1.15m)
            => Math.Round(v * factor, 2, MidpointRounding.AwayFromZero);


        var suspendedMatchId = matches.Last().Id;

        var regularByMatch = new Dictionary<Guid, (Offer oneXtwo, Offer doubleChance)>();
        var regularOddsByOffer = new Dictionary<Guid, Dictionary<Guid, decimal>>(); 

        foreach (var match in matches)
        {
            var offer1x2 = new Offer
            {
                MatchId = match.Id,
                MarketTemplateId = m1x2.Id,
                Category = OfferCategory.Regular,
                ValidFromUtc = now.AddHours(-3)
            };
            var offerDC = new Offer
            {
                MatchId = match.Id,
                MarketTemplateId = mdc.Id,
                Category = OfferCategory.Regular,
                ValidFromUtc = now.AddHours(-3)
            };

            db.Offers.AddRange(offer1x2, offerDC);
            await db.SaveChangesAsync(ct); 

            var odds1x2 = new Dictionary<Guid, decimal>
            {
                [o1.Id] = Rand(1.30m, 2.60m),
                [oX.Id] = Rand(2.40m, 4.40m),
                [o2.Id] = Rand(1.60m, 3.90m)
            };
            var oddsDC = new Dictionary<Guid, decimal>
            {
                [o1x.Id] = Rand(1.20m, 1.90m),
                [ox2.Id] = Rand(1.20m, 1.90m),
                [o12.Id] = Rand(1.10m, 1.70m)
            };

            bool isSuspended = match.Id == suspendedMatchId;

            db.OfferOutcomes.AddRange(
                new OfferOutcome { OfferId = offer1x2.Id, OutcomeTemplateId = o1.Id, Odds = odds1x2[o1.Id], IsEnabled = !isSuspended },
                new OfferOutcome { OfferId = offer1x2.Id, OutcomeTemplateId = oX.Id, Odds = odds1x2[oX.Id], IsEnabled = !isSuspended },
                new OfferOutcome { OfferId = offer1x2.Id, OutcomeTemplateId = o2.Id, Odds = odds1x2[o2.Id], IsEnabled = !isSuspended },

                new OfferOutcome { OfferId = offerDC.Id, OutcomeTemplateId = o1x.Id, Odds = oddsDC[o1x.Id], IsEnabled = !isSuspended },
                new OfferOutcome { OfferId = offerDC.Id, OutcomeTemplateId = ox2.Id, Odds = oddsDC[ox2.Id], IsEnabled = !isSuspended },
                new OfferOutcome { OfferId = offerDC.Id, OutcomeTemplateId = o12.Id, Odds = oddsDC[o12.Id], IsEnabled = !isSuspended }
            );
            await db.SaveChangesAsync(ct);

            regularByMatch[match.Id] = (offer1x2, offerDC);
            regularOddsByOffer[offer1x2.Id] = odds1x2;
            regularOddsByOffer[offerDC.Id] = oddsDC;
        }


        var topMatches = matches.Take(4).Where(m => m.Id != suspendedMatchId).ToList();
        foreach (var match in topMatches)
        {
            var (reg1x2, regDC) = regularByMatch[match.Id];
            var reg1x2Odds = regularOddsByOffer[reg1x2.Id];
            var regDCOdds = regularOddsByOffer[regDC.Id];

            var top1x2 = new Offer
            {
                MatchId = match.Id,
                MarketTemplateId = m1x2.Id,
                Category = OfferCategory.Top,
                ValidFromUtc = now.AddHours(-2)
            };
            var topDC = new Offer
            {
                MatchId = match.Id,
                MarketTemplateId = mdc.Id,
                Category = OfferCategory.Top,
                ValidFromUtc = now.AddHours(-2)
            };

            db.Offers.AddRange(top1x2, topDC);
            await db.SaveChangesAsync(ct);

            db.OfferOutcomes.AddRange(
                new OfferOutcome { OfferId = top1x2.Id, OutcomeTemplateId = o1.Id, Odds = Bump(reg1x2Odds[o1.Id]), IsEnabled = true },
                new OfferOutcome { OfferId = top1x2.Id, OutcomeTemplateId = oX.Id, Odds = Bump(reg1x2Odds[oX.Id]), IsEnabled = true },
                new OfferOutcome { OfferId = top1x2.Id, OutcomeTemplateId = o2.Id, Odds = Bump(reg1x2Odds[o2.Id]), IsEnabled = true },

                new OfferOutcome { OfferId = topDC.Id, OutcomeTemplateId = o1x.Id, Odds = Bump(regDCOdds[o1x.Id]), IsEnabled = true },
                new OfferOutcome { OfferId = topDC.Id, OutcomeTemplateId = ox2.Id, Odds = Bump(regDCOdds[ox2.Id]), IsEnabled = true },
                new OfferOutcome { OfferId = topDC.Id, OutcomeTemplateId = o12.Id, Odds = Bump(regDCOdds[o12.Id]), IsEnabled = true }
            );

            await db.SaveChangesAsync(ct);
        }

        logger.LogInformation("Initial seed applied: 10 matches. All have REGULAR (1X2, DoubleChance); first 4 also have TOP with higher odds; last match suspended (all outcomes disabled).");

        if (!await db.Players.AnyAsync(ct))
        {
            var player = new Player { DisplayName = "InitialUser" };
            db.Players.Add(player);
            await db.SaveChangesAsync(ct);

            var wallet = new Wallet { PlayerId = player.Id, Balance = 0m };
            db.Wallets.Add(wallet);
            await db.SaveChangesAsync(ct);

            const decimal initial = 20m;
            db.WalletTransactions.Add(new WalletTransaction
            {
                WalletId = wallet.Id,
                OccurredAtUtc = DateTime.UtcNow,
                Type = WalletTransactionType.Deposit,
                Amount = initial,
                Note = "Initial seed deposit"
            });
            wallet.Balance += initial;

            await db.SaveChangesAsync(ct);
        }
    }
}
