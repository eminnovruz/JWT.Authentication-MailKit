using Microsoft.AspNetCore.Mvc;
using WebApi.DataTransferObject.Request;
using WebApi.Services.Abstract;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VerificationController : ControllerBase
{
    private readonly IVerificationService _verificationService;

    public VerificationController(IVerificationService verificationService)
    {
        _verificationService = verificationService;
    }

    [HttpGet("verifyemail")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string code)
    {
        try
        {
            var request = new VerifyEmailRequest { Email = email, Code = code };
            var isVerified = await _verificationService.VerifyEmailVerificationCode(request);

            if (isVerified)
            {
                return Ok("Email verified successfully.");
            }

            return BadRequest("Invalid verification code or code has expired.");
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
