using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailNotifier : IAlertNotifier
{
    private readonly IConfiguration _cfg;
    private readonly string _smtpHost;
    private readonly int _smtpPort;

    public EmailNotifier(IConfiguration cfg)
    {
        _cfg = cfg;
        _smtpHost = _cfg["Email:SmtpHost"] ?? "localhost";
        _smtpPort = int.Parse(_cfg["Email:SmtpPort"] ?? "25");
    }

    public Task NotifyAsync(string sensorCode, string? equipmentInfo, string subject, string body)
    {
        // para simplicidade: usa SmtpClient direto. Para produção, integrar SendGrid/SES, etc.
        using var client = new SmtpClient(_smtpHost, _smtpPort);
        var msg = new MailMessage("alerts@monitor.app", "client@example.com", subject, body);
        client.Send(msg);
        return Task.CompletedTask;
    }
}
