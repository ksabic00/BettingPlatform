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
        var matches = new List<Match>();
        for (int i = 0; i < 11; i++)
        {
            matches.Add(new Match
            {
                HomeTeam = $"Team{i + 1}A",
                AwayTeam = $"Team{i + 1}B",
                StartsAtUtc = now.AddDays((i % 5) + 1),
                Sport = "football",
                Status = null
            });
        }
        db.Matches.AddRange(matches);
        await db.SaveChangesAsync(ct);


        var offersByMatch = new Dictionary<Guid, (Offer OneXTwo, Offer DoubleChance)>();
        for (int i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var category = (i < 4) ? OfferCategory.Top : OfferCategory.Regular;

            var offer1x2 = new Offer
            {
                MatchId = match.Id,
                MarketTemplateId = m1x2.Id,
                Category = category,
                ValidFromUtc = now.AddHours(-2),
                ValidToUtc = null
            };
            var offerDC = new Offer
            {
                MatchId = match.Id,
                MarketTemplateId = mdc.Id,
                Category = category,
                ValidFromUtc = now.AddHours(-2),
                ValidToUtc = null
            };

            db.Offers.AddRange(offer1x2, offerDC);
            offersByMatch[match.Id] = (offer1x2, offerDC);
        }
        await db.SaveChangesAsync(ct);

        var rnd = new Random(20251014);
        decimal NextOdd(decimal min, decimal max)
        {
            var val = (decimal)rnd.NextDouble() * (max - min) + min;
            return Math.Round(val, 2, MidpointRounding.AwayFromZero);
        }

        var allOfferOutcomes = new List<OfferOutcome>();

        for (int i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var (offer1x2, offerDC) = offersByMatch[match.Id];

            var oo1_1 = new OfferOutcome { OfferId = offer1x2.Id, OutcomeTemplateId = o1.Id, Odds = NextOdd(1.30m, 2.60m), IsEnabled = true };
            var oo1_X = new OfferOutcome { OfferId = offer1x2.Id, OutcomeTemplateId = oX.Id, Odds = NextOdd(2.40m, 4.40m), IsEnabled = true };
            var oo1_2 = new OfferOutcome { OfferId = offer1x2.Id, OutcomeTemplateId = o2.Id, Odds = NextOdd(1.60m, 3.90m), IsEnabled = true };

            var ooDC_1x = new OfferOutcome { OfferId = offerDC.Id, OutcomeTemplateId = o1x.Id, Odds = NextOdd(1.20m, 1.90m), IsEnabled = true };
            var ooDC_x2 = new OfferOutcome { OfferId = offerDC.Id, OutcomeTemplateId = ox2.Id, Odds = NextOdd(1.20m, 1.90m), IsEnabled = true };
            var ooDC_12 = new OfferOutcome { OfferId = offerDC.Id, OutcomeTemplateId = o12.Id, Odds = NextOdd(1.10m, 1.70m), IsEnabled = true };

            if (i == 10)
            {
                oo1_1.IsEnabled = false;
                oo1_X.IsEnabled = false;
                oo1_2.IsEnabled = false;
                ooDC_1x.IsEnabled = false;
                ooDC_x2.IsEnabled = false;
                ooDC_12.IsEnabled = false;
            }

            allOfferOutcomes.AddRange(new[] { oo1_1, oo1_X, oo1_2, ooDC_1x, ooDC_x2, ooDC_12 });
        }

        db.OfferOutcomes.AddRange(allOfferOutcomes);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Initial seed applied: 11 matches (4 TOP, 6 REGULAR, 1 DISABLED), each with 1X2 and DoubleChance; disabled match has all outcomes IsEnabled=false.");

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
