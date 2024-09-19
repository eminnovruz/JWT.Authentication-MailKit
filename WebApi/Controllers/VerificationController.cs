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

    // Endpoint to verify an email code and get an access token
    [HttpPost("verifyAndGetAccessToken")]
    public async Task<IActionResult> VerifyAndGetAccessToken(VerifyEmailRequest request)
    {
        try
        {
            var token = await _verificationService.VerifyCodeAndGetToken(request);
            return Ok(token);
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    // Endpoint to verify a TOTP code and get an access token
    [HttpPost("verifyTotpAndGetAccessToken")]
    public async Task<IActionResult> VerifyTotpAndGetAccessToken(VerifyTotpRequest request)
    {
        try
        {
            var token = await _verificationService.VerifyTotpAndGetToken(request);
            return Ok(token);
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
