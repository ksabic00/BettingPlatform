namespace BettingPlatform.Application.Players.Dtos;
public class PlayerSummaryDto
{
    public Guid Id { get; init; }
    public string DisplayName { get; init; } = default!;
}
