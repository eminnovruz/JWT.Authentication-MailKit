using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataTransferObject.Request;
using WebApi.Services;
using WebApi.Services.Abstract;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IAuthService _authSerivce;

        public ApplicationController(IAuthService authSerivce)
        {
            _authSerivce = authSerivce;
        }

        [HttpPost("register/user")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            try
            {
                return Ok(await _authSerivce.Register(request));
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("login/user")]
        public async Task<IActionResult> Login(LoginUserRequest request)
        {
            try
            {
                return Ok(await _authSerivce.Login(request));
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [Authorize(Roles = "User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPatch("enable/twofactorauth")]
        public async Task<IActionResult> EnableTwoFactorAuth(EnableTwoFactorAuthRequest request)
        {
            try
            {
                var result = await _authSerivce.EnableTwoFactorAuth(request);
                return Ok(result);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
