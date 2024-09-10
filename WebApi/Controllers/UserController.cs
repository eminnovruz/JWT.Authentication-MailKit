using Microsoft.AspNetCore.Mvc;
using WebApi.DataTransferObject.Request;
using WebApi.Services.Abstract;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
	private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("registerUser")]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
    {
		try
		{
            var result = await _userService.Register(request);

            if (result is true)
                return Ok("Check your inbox and verify email address.");

            return BadRequest("Something went wrong, try again later.");
		}
		catch (Exception exception)
		{
			return BadRequest(exception.Message);
		}
    }

    [HttpPost("change2FA")]
    public async Task<IActionResult> Change2FA(EnableTwoFactorAuthRequest request)
    {
        try
        {
            var result = await _userService.EnableTwoFactorAuth(request);

            return Ok(result);
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("")]
}
