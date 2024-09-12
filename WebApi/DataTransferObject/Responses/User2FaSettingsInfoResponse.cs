namespace WebApi.DataTransferObject.Responses;

public class User2FaSettingsInfoResponse
{
    public bool TwoFactorAuthentication { get; set; }
    public string TwoFactorAuthenticationType { get; set; }
}
