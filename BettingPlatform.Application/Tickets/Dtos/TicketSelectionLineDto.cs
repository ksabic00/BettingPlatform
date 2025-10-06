using BettingPlatform.Domain.Enums;

namespace BettingPlatform.Application.Tickets.Dtos;

public class TicketSelectionLineDto
{
    public string HomeTeam { get; init; } = default!;
    public string AwayTeam { get; init; } = default!;
    public string OutcomeCode { get; init; } = default!;
    public decimal OddAtPlacement { get; init; }
    public OfferCategory CategoryAtPlacement { get; init; }
}
