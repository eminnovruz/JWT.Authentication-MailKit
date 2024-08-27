using WebApi.DataTransferObject.Request;
using WebApi.DataTransferObject.Responses;

namespace WebApi.Services.Abstract;

public interface IAuthService
{
    Task<bool> RegisterUser(RegisterUserRequest request);
    Task<AuthTokenInfoResponse> Login(LoginUserRequest request);
}
