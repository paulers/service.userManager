namespace Service.UserManager.Services;

using System.Net;
using System.Net.Mail;

public interface IEmailService
{
    Task<bool> SendEmail(MailAddress to, MailAddress from, string body, string subject);
}

public class EmailService : IEmailService
{
    private string _smtpHost;
    private int _smtpPort;
    private string _smtpUser;
    private string _smtpPass;

    public EmailService(IConfiguration config)
    {
        _smtpHost = config.GetValue<string>("Mail:Host");
        _smtpPort = config.GetValue<int>("Mail:Port");
        _smtpUser = config.GetValue<string>("Mail:User");
        _smtpPass = config.GetValue<string>("Mail:Pass");
    }

    public async Task<bool> SendEmail(MailAddress to, MailAddress from, string body, string subject)
    {
        ArgumentNullException.ThrowIfNull(to);
        ArgumentNullException.ThrowIfNull(from);

        var message = new MailMessage(from, to);

        using var client = new SmtpClient(_smtpHost, _smtpPort);
        client.UseDefaultCredentials = false;
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);

        message.Body = body;
        message.BodyEncoding = System.Text.Encoding.UTF8;
        message.Subject = subject;
        message.SubjectEncoding = System.Text.Encoding.UTF8;

        await client.SendMailAsync(message);

        message.Dispose();
        return true;
    }
}
