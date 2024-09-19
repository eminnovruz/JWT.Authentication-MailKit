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

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

        try
        {
            // Validate token with issuer signing key and disable lifetime validation (so it can parse expired tokens)
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // Ignore token expiration
                ClockSkew = TimeSpan.Zero // Remove default 5 min buffer
            }, out SecurityToken securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch
        {
            // Return null if the token validation fails
            return null;
        }
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
