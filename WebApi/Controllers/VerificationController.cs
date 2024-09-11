using Microsoft.AspNetCore.Mvc;
using WebApi.DataTransferObject.Request;
using WebApi.Models;
using WebApi.Services;
using WebApi.Services.Abstract;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VerificationController : ControllerBase
{
    private readonly IVerificationService _verificationService;

    public VerificationController(IVerificationService verificationService, IJwtService jwtService)
    {
        _verificationService = verificationService;
    }

    
    [HttpPost("verifyAndGetAccessToken")]
    public async Task<IActionResult> VerifyAndGetAccessToken(VerifyEmailRequest request)
    {
        try
        {
            return Ok(await _verificationService.VerifyCodeAndGetToken(request));
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
