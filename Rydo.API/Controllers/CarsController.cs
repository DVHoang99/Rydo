using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rydo.Application.Cars.Commands;
using Rydo.Application.Cars.Queries;

namespace Rydo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController: ControllerBase
{
    private readonly IMediator _mediator;

    public CarsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    //[Authorize]
    public async Task<IActionResult> Create(CreateCarCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return Ok(new { id });
    }
    
    [HttpGet("search")]
    //[Authorize]
    public async Task<IActionResult> Search(
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        [FromQuery] double? maxDistanceKm,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? brand,
        [FromQuery] string? model)
    {
        var query = new SearchCarsQuery(latitude, longitude, maxDistanceKm, minPrice, maxPrice, brand, model);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}