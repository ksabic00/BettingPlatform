using BettingPlatform.Application.Tickets.Dtos;
using MediatR;

namespace BettingPlatform.Application.Tickets.Commands.PlaceTicket;

public sealed class PlaceTicketCommand : IRequest<TicketPlacedDto>
{
    public Guid PlayerId { get; init; }
    public decimal Stake { get; init; }
    public IReadOnlyList<PlaceTicketSelectionDto> Selections { get; init; } = Array.Empty<PlaceTicketSelectionDto>();
}
