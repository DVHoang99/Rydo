using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rydo.Application.Cars.Commands;

namespace Rydo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController: ControllerBase
{
    private readonly IMediator _mediator;

    public CarsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateCarCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return Ok(new { id });
    }
}