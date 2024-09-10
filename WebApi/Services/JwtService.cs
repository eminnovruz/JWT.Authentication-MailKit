using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApi.DataTransferObject.Responses;
using WebApi.Services.Abstract;

namespace WebApi.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AuthTokenInfoResponse GenerateSecurityToken(string id, string email, string role)
    {
        // Retrieve key and other JWT settings from configuration
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenHandler = new JwtSecurityTokenHandler();
        var now = DateTime.UtcNow;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, id),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role)
            }),
            NotBefore = now, // Token is valid from now
            Expires = now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpiresDate"])), // Token lifetime from appsettings
            SigningCredentials = creds,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        // Generate refresh token (optional)
        var refreshToken = Guid.NewGuid().ToString();

        return new AuthTokenInfoResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpireDate = tokenDescriptor.Expires ?? now.AddHours(12)
        };
    }
}
