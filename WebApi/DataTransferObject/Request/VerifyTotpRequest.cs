namespace WebApi.DataTransferObject.Request;

public class VerifyTotpRequest
{
    public string Email { get; set; }
    public string TotpCode { get; set; }
}
