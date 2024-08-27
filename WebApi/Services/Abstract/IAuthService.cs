namespace WebApi.Services.Abstract;

public interface IAuthService
{
    Task<bool> RegisterUser();
    Task<bool> Login();
}
