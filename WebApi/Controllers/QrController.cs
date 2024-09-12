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

        // Endpoint to verify the QR code (you may use this for 2FA or verification purposes)
    [HttpPost("2fa/qr-code")]
    public async Task<IActionResult> GenerateQrCode(GenerateQrCodeRequest request)
    {
        try
        {
            string secret = _qrService.GenerateSecretKey(); // Generating secret key

            await _secretKeyService.SaveUserSecret(secret, request.Email); // and saving user's secret to database

            var result = await _qrService.GenerateQrCodeAsync(request.Email, await _secretKeyService.GetUserSecret(request.Email));

            return Ok(new { result, secret });
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}