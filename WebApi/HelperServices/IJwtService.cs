using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Configuration.JWT;
using WebApi.DataTransferObject.Responses;
using WebApi.HelperServices.Abstract;

namespace WebApi.HelperServices;

public class JwtService : IJwtService
{
    private readonly JwtConfiguration _config;

    public JwtService(JwtConfiguration config)
    {
        _config = config;
    }

    public AuthTokenInfoResponse GenerateSecurityToken(string id, string email, string role)
    {
        var authTokenDto = new AuthTokenInfoResponse();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]{
            new Claim(ClaimsIdentity.DefaultNameClaimType, email),
            new Claim("userId", id),
            new Claim(ClaimTypes.Role , role)
        };

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            expires: DateTime.Now.AddHours(_config.ExpiresDate),
            notBefore: DateTime.Now,
            claims: claims,
            signingCredentials: signingCredentials
            );

        authTokenDto.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

        byte[] numbers = new byte[32];
        using RandomNumberGenerator random = RandomNumberGenerator.Create();
        random.GetBytes(numbers);

        authTokenDto.RefreshToken = Convert.ToBase64String(numbers);
        authTokenDto.ExpireDate = DateTime.Now.AddHours(_config.ExpiresDate).AddMinutes(10);

        return authTokenDto;
    }
}
