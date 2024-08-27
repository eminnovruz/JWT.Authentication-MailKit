using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;
using WebApi.DataTransferObject.Request;
using WebApi.Services.Abstract;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("RegisterUser")]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
    {
        try
        {
            return Ok(await _authService.Register(request));
        }
        catch (Exception exception )
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("LoginUser")]
    public async Task<IActionResult> LoginUser(LoginUserRequest request)
    {
        try
        {
            return Ok(await _authService.Login(request));
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

}
