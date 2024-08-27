namespace WebApi.DataTransferObject.Responses;

public class AuthTokenInfoResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpireDate { get; set; }
}
