namespace WebApi.DataTransferObject.Request;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
    public DateTime ExpireDate { get; set; }
}
