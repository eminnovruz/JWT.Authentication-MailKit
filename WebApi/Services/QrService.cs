using OtpNet;
using QRCoder;
using WebApi.Services.Abstract;

namespace WebApi.Services;

public class QrService : IQrService
{
    public string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    public string GenerateQrCodeUri(string email, string secretKey, string issuer = "YourAppName")
    {
        return $"otpauth://totp/{issuer}:{email}?secret={secretKey}&issuer={issuer}&digits=6";
    }

    public async Task<string> GenerateQrCodeAsync(string email, string secretKey, string issuer = "YourAppName")
    {
        var uri = GenerateQrCodeUri(email, secretKey, issuer);

        return await Task.Run(() =>
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new PngByteQRCode(qrCodeData))
                {
                    var qrCodeBytes = qrCode.GetGraphic(20);
                    return Convert.ToBase64String(qrCodeBytes);
                }
            }
        });
    }

    public bool VerifyTotpCode(string secretKey, string userInputCode)
    {
        var key = Base32Encoding.ToBytes(secretKey);
        var totp = new Totp(key);
        return totp.VerifyTotp(userInputCode, out long timeStepMatched, new VerificationWindow(2, 2));
    }
}
