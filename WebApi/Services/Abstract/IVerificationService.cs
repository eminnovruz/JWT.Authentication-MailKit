using WebApi.DataTransferObject.Request;

namespace WebApi.Services.Abstract;

public interface IVerificationService
{
    Task<bool> SendVerificationEmail(string email);
    Task<bool> VerifyCode(VerifyEmailRequest request);
}
