using BettingPlatform.Application.Common.Interfaces;
using BettingPlatform.Application.Tickets.Dtos;
using BettingPlatform.Domain.Entities;
using BettingPlatform.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BettingPlatform.Application.Tickets.Commands.PlaceTicket;

public sealed class PlaceTicketHandler : IRequestHandler<PlaceTicketCommand, TicketPlacedDto>
{
    private readonly IAppDbContext _db;
    public PlaceTicketHandler(IAppDbContext db) => _db = db;

    public async Task<TicketPlacedDto> Handle(PlaceTicketCommand cmd, CancellationToken ct)
    {
        var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.PlayerId == cmd.PlayerId, ct)
                     ?? throw new KeyNotFoundException("Wallet not found.");

        if (cmd.Selections.Count == 0)
            throw new FluentValidation.ValidationException("At least one selection is required.");

        var now = DateTime.UtcNow;

        var offerIds = cmd.Selections.Select(s => s.OfferId).Distinct().ToList();
        var outcomeIds = cmd.Selections.Select(s => s.OutcomeTemplateId).Distinct().ToList();

        var offers = await _db.Offers
            .Where(o => offerIds.Contains(o.Id))
            .Select(o => new { o.Id, o.MatchId, o.Category, o.ValidFromUtc, o.ValidToUtc })
            .ToListAsync(ct);
        if (offers.Count != offerIds.Count)
            throw new FluentValidation.ValidationException("Some offers do not exist.");

        var offerById = offers.ToDictionary(x => x.Id, x => x);

        foreach (var off in offers)
        {
            var active = off.ValidFromUtc <= now && (off.ValidToUtc == null || off.ValidToUtc >= now);
            if (!active) throw new FluentValidation.ValidationException("Offer is not active.");
        }

        var offerOutcomes = await _db.OfferOutcomes
            .Where(oo => offerIds.Contains(oo.OfferId) && outcomeIds.Contains(oo.OutcomeTemplateId))
            .ToListAsync(ct);

        var outcomeByKey = offerOutcomes.ToDictionary(x => (x.OfferId, x.OutcomeTemplateId), x => x);


        foreach (var s in cmd.Selections)
        {
            if (!outcomeByKey.TryGetValue((s.OfferId, s.OutcomeTemplateId), out var oo))
                throw new FluentValidation.ValidationException("Selected outcome does not exist for the given offer.");
            if (!oo.IsEnabled)
                throw new FluentValidation.ValidationException("Selected outcome is disabled.");
        }

        var topCount = cmd.Selections.Count(s => offerById[s.OfferId].Category == OfferCategory.Top);
        if (topCount > 1)
            throw new FluentValidation.ValidationException("You cannot combine more than one TOP offer.");

        var matchCounts = cmd.Selections
            .Select(s => offerById[s.OfferId].MatchId)
            .GroupBy(m => m)
            .ToDictionary(g => g.Key, g => g.Count());
        if (matchCounts.Values.Any(c => c > 1))
            throw new FluentValidation.ValidationException("You cannot add the same match more than once.");

        var fee = Math.Round(cmd.Stake * 0.05m, 2, MidpointRounding.AwayFromZero);
        var stakeNet = cmd.Stake - fee;

        decimal combinedOdds = 1m;
        foreach (var s in cmd.Selections)
            combinedOdds *= outcomeByKey[(s.OfferId, s.OutcomeTemplateId)].Odds;
        combinedOdds = Math.Round(combinedOdds, 2, MidpointRounding.AwayFromZero);

        var potential = Math.Round(stakeNet * combinedOdds, 2, MidpointRounding.AwayFromZero);

        if (wallet.Balance < cmd.Stake)
            throw new FluentValidation.ValidationException("Insufficient funds.");

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var ticketId = Guid.NewGuid();
        var ticket = new Ticket
        {
            Id = ticketId,
            PlayerId = cmd.PlayerId,
            StakeGross = cmd.Stake,
            FeePercent = 0.05m,
            FeeAmount = fee,
            StakeNet = stakeNet,
            CombinedOdds = combinedOdds,
            PotentialPayout = potential,
            CreatedAtUtc = DateTime.UtcNow,
            Status = TicketStatus.Placed
        };
        _db.Tickets.Add(ticket);

        foreach (var s in cmd.Selections)
        {
            var offer = offerById[s.OfferId];
            var odd = outcomeByKey[(s.OfferId, s.OutcomeTemplateId)].Odds;

            _db.TicketSelections.Add(new TicketSelection
            {
                TicketId = ticketId,
                OfferId = s.OfferId,
                MatchId = offer.MatchId,
                OutcomeTemplateId = s.OutcomeTemplateId,
                OddAtPlacement = odd,
                CategoryAtPlacement = offer.Category
            });
        }

        _db.WalletTransactions.Add(new WalletTransaction
        {
            WalletId = wallet.Id,
            OccurredAtUtc = DateTime.UtcNow,
            Type = WalletTransactionType.BetStakeDebit,
            Amount = -stakeNet,
            ReferenceId = ticketId.ToString(),
            Note = "Ticket stake"
        });
        _db.WalletTransactions.Add(new WalletTransaction
        {
            WalletId = wallet.Id,
            OccurredAtUtc = DateTime.UtcNow,
            Type = WalletTransactionType.BetFeeDebit,
            Amount = -fee,
            ReferenceId = ticketId.ToString(),
            Note = "Ticket fee 5%"
        });
        wallet.Balance -= cmd.Stake;

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new TicketPlacedDto
        {
            TicketId = ticketId,
            StakeGross = cmd.Stake,
            FeeAmount = fee,
            StakeNet = stakeNet,
            CombinedOdds = combinedOdds,
            PotentialPayout = potential,
            SelectionCount = cmd.Selections.Count,
            BalanceAfter = wallet.Balance
        };
    }
}
