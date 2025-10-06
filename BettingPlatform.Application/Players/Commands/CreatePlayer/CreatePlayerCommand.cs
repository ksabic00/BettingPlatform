using MediatR;

namespace BettingPlatform.Application.Players.Commands.CreatePlayer;

public sealed class CreatePlayerCommand : IRequest<Guid>
{
    public string DisplayName { get; init; } = default!;
}
