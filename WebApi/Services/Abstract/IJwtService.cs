using WebApi.DataTransferObject.Responses;

namespace WebApi.Services.Abstract;

public interface IJwtService
{
    AuthTokenInfoResponse GenerateSecurityToken(string id, string email, string role);
}
