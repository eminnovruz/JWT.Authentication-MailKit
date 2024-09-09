namespace WebApi.DataTransferObject.Request;

public class VerifyEmailRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
}
