using BettingPlatform.Application.Tickets.Dtos;
using MediatR;

namespace BettingPlatform.Application.Tickets.Queries.GetTicketDetails;

public sealed class GetTicketDetailsQuery : IRequest<TicketDetailDto>
{
    public Guid TicketId { get; }
    public GetTicketDetailsQuery(Guid ticketId) => TicketId = ticketId;
}
