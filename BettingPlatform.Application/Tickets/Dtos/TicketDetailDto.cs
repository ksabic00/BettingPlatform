namespace BettingPlatform.Application.Tickets.Dtos;

public class TicketDetailDto
{
    public Guid TicketId { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public decimal StakeGross { get; init; }
    public decimal StakeNet { get; init; }
    public decimal CombinedOdds { get; init; }
    public decimal PotentialPayout { get; init; }
    public string Status { get; init; } = default!;
    public IReadOnlyList<TicketSelectionLineDto> Selections { get; init; } = Array.Empty<TicketSelectionLineDto>();
}
