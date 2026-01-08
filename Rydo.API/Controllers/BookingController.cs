using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rydo.Application.Bookings.Commands;
using Rydo.Application.Cars.Commands;
using Rydo.Application.Cars.Queries;

namespace Rydo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Booking(CreateBookingCommand cmd)
    {
        var result = await mediator.Send(cmd);
        return Ok(result);
    }
    
    [HttpPost("cancel")]
    [Authorize]
    public async Task<IActionResult> Booking(CancelBookingCommand cmd)
    {
        var result = await mediator.Send(cmd);
        return Ok(result);
    }
}