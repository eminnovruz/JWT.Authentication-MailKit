namespace WebApi.Services.Abstract;

public interface ISecretKeyService
{
    Task<bool> SaveUserSecret(string secret, string email);
    Task<string> GetUserSecret(string email);
}
