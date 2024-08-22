using WebApi.Configuration.Mail;

namespace WebApi.Services.Abstract;

public interface IMailService
{
    Task SendEmailAsync(string to, string subject, string htmlContent);
}
