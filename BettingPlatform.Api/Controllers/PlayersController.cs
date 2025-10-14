using BettingPlatform.Application.Players.Commands.CreatePlayer;
using BettingPlatform.Application.Players.Dtos;
using BettingPlatform.Application.Players.Queries.ListPlayers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BettingPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public Task<Guid> Create([FromBody] CreatePlayerCommand cmd)
        => mediator.Send(cmd);

    [HttpGet]
    public Task<IReadOnlyList<PlayerSummaryDto>> List()
    => mediator.Send(new ListPlayersQuery());
}
