using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rydo.Application.Accounts.Commands;

namespace Rydo.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand request)
    {
        var token = await mediator.Send(request);
        return Ok(new { Token = token });
    }
}