using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    [HttpPost("loginUser")]
    public async Task<IActionResult> LoginUser(LoginUserRequest request)
    {
        try
        {
            var result = await _userService.Login(request);

            if (result is null)
                return Ok("Check your inbox to verify email address");

            return Ok(await _userService.Login(request));   
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("setPassword")]
    public async Task<IActionResult> SetPassword(SetUserPasswordRequest request)
    {
        try
        {
            return Ok(await _userService.SetUserPassword(request));
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
