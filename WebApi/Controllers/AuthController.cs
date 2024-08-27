using Microsoft.AspNetCore.Mvc;
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
            return Ok(await _authService.RegisterUser(request));
        }
        catch (Exception exception )
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("LoginUser")]

}
