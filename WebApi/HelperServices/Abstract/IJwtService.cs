using WebApi.DataTransferObject.Responses;

namespace WebApi.HelperServices.Abstract;

public interface IJwtService
{
    AuthTokenInfoResponse GenerateSecurityToken(string id, string email, string role);
}
