using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rydo.Application.Accounts.Commands;

namespace Rydo.API.Controllers;

public class AccountController(IMediator mediator) : ControllerBase
{
    [HttpPost("Register")]
    //[Authorize]
    public async Task<IActionResult> Register(RegisterCommand cmd)
    {
        var result = await mediator.Send(cmd);
        return Ok(result);
    }
    
    [HttpPost("Login")]
    //[Authorize]
    public async Task<IActionResult> Login(LoginCommand cmd)
    {
        var result = await mediator.Send(cmd);
        return Ok(result);
    }
    
    [HttpPatch]
    //[Authorize]
    public async Task<IActionResult> Update(LoginCommand cmd)
    {
        var result = await mediator.Send(cmd);
        return Ok(result);
    }
}