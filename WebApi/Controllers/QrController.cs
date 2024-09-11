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

    public QrController(IQrService qrService)
    {
        _qrService = qrService;
    }

    [HttpPost("2fa/verify")]
    public IActionResult VerifyTotpCode(VerifyTotpRequest request)
    {
        var secretKey = ""; // Retrieve user's secret key
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

        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

}