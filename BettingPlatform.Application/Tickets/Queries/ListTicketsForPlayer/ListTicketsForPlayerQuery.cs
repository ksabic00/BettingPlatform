using BettingPlatform.Application.Tickets.Dtos;
using MediatR;

namespace BettingPlatform.Application.Tickets.Queries.ListTicketsForPlayer;

public sealed class ListTicketsForPlayerQuery : IRequest<IReadOnlyList<TicketSummaryDto>>
{
    public Guid PlayerId { get; }
    public ListTicketsForPlayerQuery(Guid playerId) => PlayerId = playerId;
}
