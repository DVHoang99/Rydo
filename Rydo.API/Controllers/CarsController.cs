using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rydo.Application.Cars.Commands;
using Rydo.Application.Cars.Queries;

namespace Rydo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    //[Authorize]
    public async Task<IActionResult> Create(CreateCarCommand cmd)
    {
        var id = await mediator.Send(cmd);
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
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("/{id}")]
    //[Authorize]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var result = await mediator.Send(new GetCarDetailQuery(id));
        return Ok(result);
    }
    
    [HttpGet("/{id}/availability")]
    //[Authorize]
    public async Task<IActionResult> GetCaravAilability(Guid id, DateTime from, DateTime to)
    {
        var result = await mediator.Send(new GetCarAvailabilityQuery(id, from, to));
        return Ok(result);
    }
}