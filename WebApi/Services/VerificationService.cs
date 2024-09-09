using MongoDB.Driver;
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

    public async Task<bool> RequireSetPassword(string email)
    {
        var templatePath = Path.Combine(_webRootPath, "send-navigatebutton.html");
        var templateContent = await File.ReadAllTextAsync(templatePath);

        var emailContent = templateContent.Replace("{{ActionUrl}}", "");

        return await _mailService.SendEmailAsync(email, "Password Required", emailContent);
    }

    public async Task<bool> SendEmailVerificationCode(SendEmailVerificationCodeRequest request)
    {
        var verificationCode = new Random().Next(100000, 999999).ToString();

        var templatePath = Path.Combine(_webRootPath, "send-verificationcode.html");
        var templateContent = await File.ReadAllTextAsync(templatePath);

        var emailContent = templateContent.Replace("{{VerificationCode}}", verificationCode);
        await _mailService.SendEmailAsync(request.Email, "Email Verification 🔥", emailContent);

        // Update user's verification code and expiry
        var update = Builders<User>.Update
            .Set(u => u.VerificationCode, verificationCode)
            .Set(u => u.VerificationCodeExpire, DateTime.UtcNow.AddMinutes(10));

        await _context.Users.UpdateOneAsync(u => u.Email == request.Email, update);

        return true;
    }

    public async Task<bool> VerifyEmailVerificationCode(VerifyEmailRequest request)
    {
        var user = await _context.Users
            .Find(u => u.Email == request.Email)
            .FirstOrDefaultAsync();

        if (user != null
            && user.VerificationCode == request.Code
            && user.VerificationCodeExpire > DateTime.UtcNow)
        {
            user.IsEmailConfirmed = true;

            var updateDefinition = Builders<User>.Update
                .Set(u => u.IsEmailConfirmed, true);

            var result = await _context.Users
                .UpdateOneAsync(u => u.Email == request.Email, updateDefinition);

            return result.ModifiedCount > 0;
        }

        return false;
    }

}
