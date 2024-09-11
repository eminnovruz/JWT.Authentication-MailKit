using OtpNet;

namespace WebApi.Services.Abstract;

public interface IQrService
{
    string GenerateSecretKey();
    string GenerateQrCodeUri(string email, string secretKey, string issuer = "YourAppName");
    Task<string> GenerateQrCodeAsync(string email, string secretKey, string issuer = "YourAppName");
    bool VerifyTotpCode(string secretKey, string userInputCode);
}
