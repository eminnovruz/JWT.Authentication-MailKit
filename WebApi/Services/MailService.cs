using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using WebApi.Services.Abstract;

namespace WebApi.Services;

public class MailService : IMailService
{
    private readonly MailConfiguration _config;
    private readonly IWebHostEnvironment _env;

    public MailService(IOptions<MailConfiguration> config, IWebHostEnvironment env)
    {
        _config = config.Value;
        _env = env;
    }

    public async Task<bool> SendEmailAsync(string to, string subject)
    {
        var filePath = Path.Combine(_env.WebRootPath, "email-template.html");
        var htmlContent = await File.ReadAllTextAsync(filePath);

        var emailMessage = CreateEmailMessage(to, subject, htmlContent);
          
        using var client = new SmtpClient();
        await ConnectAndSendEmailAsync(client, emailMessage);

        return true;
    }

    private MimeMessage CreateEmailMessage(string to, string subject, string htmlContent)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_config.SenderName, _config.SenderEmail));
        emailMessage.To.Add(new MailboxAddress("", to));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(TextFormat.Html) { Text = htmlContent };

        return emailMessage;
    }

    private async Task ConnectAndSendEmailAsync(SmtpClient client, MimeMessage emailMessage)
    {
        await client.ConnectAsync(_config.Server, _config.Port, _config.UseSsl);
        await client.AuthenticateAsync(_config.Username, _config.Password);
        await client.SendAsync(emailMessage);
    }
}
