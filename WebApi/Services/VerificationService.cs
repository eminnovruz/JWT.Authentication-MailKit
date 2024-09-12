using Microsoft.AspNetCore.Authentication.BearerToken;
using MimeKit.Cryptography;
using MongoDB.Driver;
using Org.BouncyCastle.Asn1.Ocsp;
using OtpNet;
using WebApi.Context;
using WebApi.DataTransferObject.Request;
using WebApi.DataTransferObject.Responses;
using WebApi.Models;
using WebApi.Services.Abstract;

namespace WebApi.Services;

public class VerificationService : IVerificationService
{
    private readonly IMailService _mailService;
    private readonly MongoDbContext _context;
    private readonly string _webRootPath;
    private readonly IJwtService _jwtService;
    private readonly ISecretKeyService _secretService;
    public VerificationService(IMailService mailService, IWebHostEnvironment env, MongoDbContext context, IJwtService jwtService, ISecretKeyService secretService)
    {
        _mailService = mailService;
        _context = context;
        _webRootPath = env.WebRootPath;
        _jwtService = jwtService;
        _secretService = secretService;
    }

    public async Task<bool> SendVerificationEmail(string email)
    {
        // getting random 6 char number for code
        var verificationCode = new Random().Next(100000, 999999).ToString();

        // getting template path from wwwroot
        var templatePath = Path.Combine(_webRootPath, "send-verificationcode.html");

        var templateContent = await File.ReadAllTextAsync(templatePath); // reading content of template

        var emailContent = templateContent.Replace("{{VerificationCode}}", verificationCode); // replacing verification code in blank template

        await _mailService.SendEmailAsync(email, "Email Verification 🔥", emailContent);

        var update = Builders<User>.Update
            .Set(u => u.VerificationCode, verificationCode)
            .Set(u => u.VerificationCodeExpire, DateTimeOffset.Now.AddMinutes(10)); // updating user credentials on database

        await _context.Users.UpdateOneAsync(u => u.Email == email, update);

        return true;
    }
    public async Task<bool> RequirePassword(string email)
    {
        // getting random 6 char number for code
        var verificationCode = new Random().Next(100000, 999999).ToString();

        // getting template path from wwwroot
        var templatePath = Path.Combine(_webRootPath, "send-navigatebutton.html");

        var templateContent = await File.ReadAllTextAsync(templatePath); // reading content of template

        await _mailService.SendEmailAsync(email, "Set your password.", templateContent);

        return true;
    }

    public async Task<AuthTokenInfoResponse> VerifyCodeAndGetToken(VerifyEmailRequest request)
    {
        var user = await _context.Users
            .Find(u => u.Email == request.Email)
            .FirstOrDefaultAsync();

        if (user == null || user.VerificationCode != request.Code)
            throw new Exception("Cannot find user related with given email");

        var accessTokenResponse = _jwtService.GenerateSecurityToken(user.Id.ToString(), user.Email, user.Role);

        // Optionally save the refresh token to the database
        user.RefreshToken = accessTokenResponse.RefreshToken;
        user.TokenExpireDate = DateTime.UtcNow.AddDays(7); // Assuming 7-day refresh token validity
        await _context.Users.ReplaceOneAsync(u => u.Email == user.Email, user);

        await RequirePassword(user.Email);

        // Return the access token, refresh token, and expiration date
        return accessTokenResponse;
    }

    public async Task<AuthTokenInfoResponse> VerifyTotpAndGetToken(VerifyTotpRequest request)
    {
        var user = await _context.Users.Find(user => user.Email == request.Email).FirstOrDefaultAsync();

        if (user is null)
            throw new Exception("Cannot find user related with given email");

        if (VerifyTotpCode(await _secretService.GetUserSecret(request.Email), request.TotpCode))
        {
            var accessTokenResponse = _jwtService.GenerateSecurityToken(user.Id.ToString(), user.Email, user.Role);

            // Optionally save the refresh token to the database
            user.RefreshToken = accessTokenResponse.RefreshToken;
            user.TokenExpireDate = DateTime.UtcNow.AddDays(7); // Assuming 7-day refresh token validity
            await _context.Users.ReplaceOneAsync(u => u.Email == user.Email, user);
            accessTokenResponse.TwoFactorAuthenticationType = user.TwoFactorAuthenticationType;

            return accessTokenResponse;
        }

        throw new Exception("Error while verifying totp code");
    }

    private bool VerifyTotpCode(string secretKey, string userInputCode)
    {
        var key = Base32Encoding.ToBytes(secretKey);
        var totp = new Totp(key);
        return totp.VerifyTotp(userInputCode, out long timeStepMatched, new VerificationWindow(2, 2));
    }
}
