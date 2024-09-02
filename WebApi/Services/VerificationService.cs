using MongoDB.Driver;
using WebApi.Context;
using WebApi.DataTransferObject.Request;
using WebApi.Services.Abstract;

namespace WebApi.Services;

public class VerificationService : IVerificationService
{
    private readonly IMailService _mailService;
    private readonly IWebHostEnvironment _env;

    private readonly MongoDbContext _context;

    public VerificationService(IMailService mailService, IWebHostEnvironment env, MongoDbContext context)
    {
        _mailService = mailService;
        _env = env;
        _context = context;
    }

    public async Task<bool> SendEmailVerificationCode(SendEmailVerificationCodeRequest request)
    {
        var verificationCode = new Random().Next(100000, 999999).ToString();

        var templatePath = Path.Combine(_env.WebRootPath, "send-verificationcode.html");
        var templateContent = await File.ReadAllTextAsync(templatePath);

        var emailContent = templateContent.Replace("{{VerificationCode}}", verificationCode);

        await _mailService.SendEmailAsync(request.Email, "Email Verification 🔥", emailContent);

        var user = await _context.Users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
        if (user != null)
        {
            user.VerificationCode = verificationCode;
            user.VerificationCodeExpire = DateTime.UtcNow.AddMinutes(10);
            await _context.Users.ReplaceOneAsync(u => u.Email == user.Email, user);
        }

        return true;
    }

    public Task<bool> VerifyEmailVerificationCode()
    {
        throw new NotImplementedException();
    }
}
