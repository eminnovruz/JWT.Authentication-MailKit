namespace WebApi.HelperServices.Abstract;

public interface ITwoFactorAuthService
{
    string GenerateSecretKey(); // No async needed, purely computational
    string GenerateQrCodeUri(string email, string secretKey, string issuer = "Erp"); // No async, string construction
    Task<string> GenerateQrCodeAsync(string email, string secretKey, string issuer = "Erp"); // Async for QR code generation
    bool VerifyTotpCode(string secretKey, string userInputCode); // No async, pure computation
}


