using WebApi.DataTransferObject.Request;

namespace WebApi.Services.Abstract;

public interface IVerificationService
{
    Task<bool> SendEmailVerificationCode(SendEmailVerificationCodeRequest request);
    Task<bool> VerifyEmailVerificationCode();
    Task<bool> RequireSetPassword(string email);
}
