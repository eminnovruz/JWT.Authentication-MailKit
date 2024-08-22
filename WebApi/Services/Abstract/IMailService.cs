namespace WebApi.Services.Abstract;

public interface IMailService
{
    Task<bool> SendEmailAsync(string to, string subject, string htmlContent);
}
