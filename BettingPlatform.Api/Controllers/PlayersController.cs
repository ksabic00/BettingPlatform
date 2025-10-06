using BettingPlatform.Application.Players.Commands.CreatePlayer;
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
}
