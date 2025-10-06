using BettingPlatform.Application.Offers.Dtos;
using BettingPlatform.Application.Offers.Queries.ListOffers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BettingPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OffersController(IMediator mediator) : ControllerBase
{
    [HttpGet("active")]
    public Task<IReadOnlyList<OfferDto>> GetActive([FromQuery] DateTime? asOfUtc)
        => mediator.Send(new ListOffersQuery(asOfUtc));
}
