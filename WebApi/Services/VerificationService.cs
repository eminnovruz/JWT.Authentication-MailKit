using MimeKit.Cryptography;
using MongoDB.Driver;
using Org.BouncyCastle.Asn1.Ocsp;
using WebApi.Context;
using WebApi.DataTransferObject.Request;
using WebApi.Models;
using WebApi.Services.Abstract;

namespace WebApi.Services;

public class VerificationService : IVerificationService
{
    private readonly IMailService _mailService;
    private readonly MongoDbContext _context;
    private readonly string _webRootPath;

    public VerificationService(IMailService mailService, IWebHostEnvironment env, MongoDbContext context)
    {
        _mailService = mailService;
        _context = context;
        _webRootPath = env.WebRootPath;
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
            .Set(u => u.VerificationCodeExpire, DateTime.UtcNow.AddMinutes(10));

        await _context.Users.UpdateOneAsync(u => u.Email == email, update);

        return true;
    }

    public Task<bool> VerifyCode(VerifyEmailRequest request)
    {
        throw new NotImplementedException();
    }
}
