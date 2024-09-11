using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataTransferObject.Request;
using WebApi.Services;
using WebApi.Services.Abstract;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QrController : ControllerBase
{
    private readonly IQrService _qrService;
    private readonly ISecretKeyService _secretKeyService;

    public QrController(IQrService qrService, ISecretKeyService secretKeyService)
    {
        _qrService = qrService;
        _secretKeyService = secretKeyService;
    }

    [HttpPost("2fa/verify")]
    public async Task<IActionResult> VerifyTotpCodeAsync(VerifyTotpRequest request)
    {
        var secretKey = await _secretKeyService.GetUserSecret(request.Email); // Retrieve user's secret key

        if (_qrService.VerifyTotpCode(secretKey, request.TotpCode))
        {
            // Code is valid
            return Ok("2FA setup successfully");
        }

        // Code is invalid
        return Unauthorized("Invalid code");
    }

    // Endpoint to verify the QR code (you may use this for 2FA or verification purposes)
    [HttpGet("2fa/qr-code")]
    public async Task<IActionResult> GenerateQrCode(string email)
    {
        try
        {
            string secret = _qrService.GenerateSecretKey();

            await _secretKeyService.SaveUserSecret(secret, email);

            var result = await _qrService.GenerateQrCodeAsync(email, await _secretKeyService.GetUserSecret(email));

            return Ok(new { result, secret });
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

}