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

    [HttpPost("verifyEmail")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request)
    {
		try
		{
            bool result = await _verificationService.VerifyCode(request);

            if (result is true)
                return Ok("Email confirmed successfully.");

            return Unauthorized("Something went wrong, try again later.");
        }
		catch (Exception exception)
		{
			return BadRequest(exception.Message);
		}
    }
}
