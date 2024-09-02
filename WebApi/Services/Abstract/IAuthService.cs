using WebApi.DataTransferObject.Request;
using WebApi.DataTransferObject.Responses;
using WebApi.Models;

namespace WebApi.Services.Abstract;

public interface IAuthService
{
    Task<bool> Register(RegisterUserRequest request);
    Task<AuthTokenInfoResponse> Login(LoginUserRequest request);
    AuthTokenInfoResponse GenerateToken(User user);
    Task<AuthTokenInfoResponse> RefreshToken(RefreshTokenRequest request);
    Task<bool> EnableTwoFactorAuth(bool flag);
}
