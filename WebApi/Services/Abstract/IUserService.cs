using WebApi.DataTransferObject.Request;
using WebApi.DataTransferObject.Responses;
using WebApi.Models;

namespace WebApi.Services.Abstract;

public interface IUserService
{
    Task<bool> Register(RegisterUserRequest request);
    Task<AuthTokenInfoResponse> Login(LoginUserRequest request);
    Task<bool> EnableTwoFactorAuth(EnableTwoFactorAuthRequest request);
    Task<bool> SetUserPassword(SetUserPasswordRequest request);
}
