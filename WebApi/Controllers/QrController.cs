using Microsoft.AspNetCore.Mvc;
using WebApi.DataTransferObject.Request;
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

    // Endpoint to generate and save the QR code for 2FA or verification purposes
    [HttpPost("2fa/qr-code")]
    public async Task<IActionResult> GenerateQrCode(GenerateQrCodeRequest request)
    {
        try
        {
            // Generate a new secret key
            string secret = _qrService.GenerateSecretKey();

            // Save the user's secret to the database
            await _secretKeyService.SaveUserSecret(secret, request.Email);

            // Generate a QR code for the user based on the saved secret key
            var result = await _qrService.GenerateQrCodeAsync(request.Email, await _secretKeyService.GetUserSecret(request.Email));

            return Ok(new { result, secret });
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
