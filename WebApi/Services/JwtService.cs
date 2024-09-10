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
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, id),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:TokenLifetime"])),
            SigningCredentials = creds,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        // Generate refresh token (optional, you can store it securely if needed)
        var refreshToken = Guid.NewGuid().ToString(); // For example, you can store this securely for token refresh logic

        return new AuthTokenInfoResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpireDate = tokenDescriptor.Expires ?? DateTime.UtcNow.AddMinutes(30) // Default to 30 mins if not set
        };
    }
}