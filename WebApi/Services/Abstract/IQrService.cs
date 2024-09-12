using OtpNet;

namespace WebApi.Services.Abstract;

public interface IQrService
{
    string GenerateSecretKey();
    string GenerateQrCodeUri(string email, string secretKey, string issuer = "Erp.gov.az");
    Task<string> GenerateQrCodeAsync(string email, string secretKey, string issuer = "Erp.gov.az");
}
