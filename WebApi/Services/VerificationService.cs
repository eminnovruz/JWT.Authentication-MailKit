using Microsoft.AspNetCore.Authentication.BearerToken;
using MimeKit.Cryptography;
using MongoDB.Driver;
using Org.BouncyCastle.Asn1.Ocsp;
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

    public VerificationService(IMailService mailService, IWebHostEnvironment env, MongoDbContext context, IJwtService jwtService)
    {
        _mailService = mailService;
        _context = context;
        _webRootPath = env.WebRootPath;
        _jwtService = jwtService;
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

    public async Task<bool> VerifyCode(VerifyEmailRequest request)
    {
        // Find the user by email
        var user = await _context.Users
            .Find(u => u.Email == request.Email)
            .FirstOrDefaultAsync();

        // If the user doesn't exist or the code doesn't match, return false
        if (user == null || user.VerificationCode != request.Code)
            return false;

        // Check if the verification code has expired
        if (user.VerificationCodeExpire < DateTime.UtcNow)
            return false;

        // Update the user's email confirmation status
        var update = Builders<User>.Update
            .Set(u => u.IsEmailConfirmed, true)
            .Set(u => u.VerificationCode, null)         // Optionally clear the verification code
            .Set(u => u.VerificationCodeExpire, default);  // Optionally clear the expiration time

        var result = await _context.Users.UpdateOneAsync(u => u.Email == request.Email, update);

        // Return true if the update was successful
        return result.ModifiedCount > 0;
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

        // Return the access token, refresh token, and expiration date
        return accessTokenResponse;
    }
}
