using BettingPlatform.Application.Tickets.Commands.PlaceTicket;
using BettingPlatform.Application.Tickets.Dtos;
using BettingPlatform.Application.Tickets.Queries.GetTicketDetails;
using BettingPlatform.Application.Tickets.Queries.ListTicketsForPlayer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BettingPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public Task<TicketPlacedDto> Place([FromBody] PlaceTicketCommand cmd)
        => mediator.Send(cmd);

    [HttpGet("player/{playerId:guid}")]
    public Task<IReadOnlyList<TicketSummaryDto>> ListForPlayer(Guid playerId)
    => mediator.Send(new ListTicketsForPlayerQuery(playerId));

    [HttpGet("{ticketId:guid}")]
    public Task<TicketDetailDto> GetById(Guid ticketId)
        => mediator.Send(new GetTicketDetailsQuery(ticketId));
}
