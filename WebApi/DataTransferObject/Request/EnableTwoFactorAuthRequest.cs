using WebApi.Models.Enums;

namespace WebApi.DataTransferObject.Request;

public class EnableTwoFactorAuthRequest
{
    public string Email { get; set; }
    public bool Flag { get; set; } 
    public TwoFactorAuthTypes AuthenticationType { get; set; }
}
