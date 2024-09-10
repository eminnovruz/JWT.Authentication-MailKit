using Microsoft.AspNetCore.Mvc;
using WebApi.DataTransferObject.Request;
using WebApi.HelperServices.Abstract;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TwoFactorAuthController : ControllerBase
{
    private readonly ITwoFactorAuthService _twoFactorAuthService;

    public TwoFactorAuthController(ITwoFactorAuthService twoFactorAuthService)
    {
        _twoFactorAuthService = twoFactorAuthService;
    }

    [HttpPost("qr-code")]
    public async Task<IActionResult> GenerateQrCodeAsync(GenerateQrCodeRequest request)
    {
        try
        {
            var secretKey = _twoFactorAuthService.GenerateSecretKey();

            var qrCodeBase64 = await _twoFactorAuthService.GenerateQrCodeAsync(request.Email, secretKey);

            return Ok(new { qrCodeBase64, secretKey });
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("verify")]
    public IActionResult VerifyTotpCode(VerifyTotpRequest request)
    {
        try
        {
            var secretKey = "storedSecret";

            if (_twoFactorAuthService.VerifyTotpCode(secretKey, request.TotpCode))
            {
                return Ok("2FA verification successfull");
            }

            return Unauthorized("Invalid Totp Code");

        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
