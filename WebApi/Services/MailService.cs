using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using WebApi.Services.Abstract;

namespace WebApi.Services;

public class MailService : IMailService
{
    private readonly MailConfiguration _config;

    public MailService(IOptions<MailConfiguration> config)
    {
        _config = config.Value;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string htmlContent)
    {
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
        try
        {
            await client.ConnectAsync(_config.Server, _config.Port, _config.UseSsl);
            await client.AuthenticateAsync(_config.Username, _config.Password);
            await client.SendAsync(emailMessage);
        }
        finally
        {
            await client.DisconnectAsync(true);
            client.Dispose();
        }
    }
}
