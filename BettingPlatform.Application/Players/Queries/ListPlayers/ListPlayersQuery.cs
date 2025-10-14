using BettingPlatform.Application.Players.Dtos;
using MediatR;

namespace BettingPlatform.Application.Players.Queries.ListPlayers;
public sealed class ListPlayersQuery : IRequest<IReadOnlyList<PlayerSummaryDto>> { }
