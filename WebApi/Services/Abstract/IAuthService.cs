using WebApi.DataTransferObject.Request;

namespace WebApi.Services.Abstract;

public interface IAuthService
{
    Task<bool> RegisterUser(RegisterUserRequest request);
    Task<bool> Login();
}
