namespace BettingPlatform.Application.Offers.Dtos;

public class OfferOutcomeDto
{
    public Guid OutcomeTemplateId { get; init; }
    public string OutcomeCode { get; init; } = default!;
    public decimal Odds { get; init; }
    public bool IsEnabled { get; init; }
}
