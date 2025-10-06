using BettingPlatform.Domain.Enums;

namespace BettingPlatform.Application.Offers.Dtos;

public class OfferDto
{
    public Guid OfferId { get; init; }
    public Guid MatchId { get; init; }
    public string HomeTeam { get; init; } = default!;
    public string AwayTeam { get; init; } = default!;
    public DateTime StartsAtUtc { get; init; }
    public string MarketCode { get; init; } = default!;
    public OfferCategory Category { get; init; }
    public IReadOnlyList<OfferOutcomeDto> Outcomes { get; init; } = new List<OfferOutcomeDto>();
}
