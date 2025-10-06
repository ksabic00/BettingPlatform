using BettingPlatform.Application.Wallets.Commands.Deposit;
using BettingPlatform.Application.Wallets.Dtos;
using BettingPlatform.Application.Wallets.Queries.GetWallet;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BettingPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController(IMediator mediator) : ControllerBase
{
    [HttpGet("{playerId:guid}")]
    public Task<WalletDto> Get(Guid playerId)
        => mediator.Send(new GetWalletQuery(playerId));

    [HttpPost("deposit")]
    public Task<WalletDto> Deposit([FromBody] DepositCommand cmd)
        => mediator.Send(cmd);
}
