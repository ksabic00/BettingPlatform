namespace BettingPlatform.Application.Tickets.Dtos;

public class TicketPlacedDto
{
    public Guid TicketId { get; init; }
    public decimal StakeGross { get; init; }
    public decimal FeeAmount { get; init; }
    public decimal StakeNet { get; init; }
    public decimal CombinedOdds { get; init; }
    public decimal PotentialPayout { get; init; }
    public int SelectionCount { get; init; }
    public decimal BalanceAfter { get; init; }
}
