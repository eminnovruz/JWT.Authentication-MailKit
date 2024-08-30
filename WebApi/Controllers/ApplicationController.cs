using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataTransferObject.Request;
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

        [HttpPatch("enable/twofactorauth")]
        public async Task<IActionResult> EnableTwoFactorAuth(bool flag)
        {
            try
            {
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
