namespace WebApi.Configuration.JWT;

public class JwtConfiguration
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiresDate { get; set; }
}
