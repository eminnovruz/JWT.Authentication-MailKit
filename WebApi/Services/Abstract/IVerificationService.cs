using WebApi.DataTransferObject.Request;
using WebApi.DataTransferObject.Responses;

namespace WebApi.Services.Abstract;

public interface IVerificationService
{
    Task<bool> SendVerificationEmail(string email);
    Task<bool> VerifyCode(VerifyEmailRequest request);
    Task<AuthTokenInfoResponse> VerifyCodeAndGetToken(VerifyEmailRequest request);
}
