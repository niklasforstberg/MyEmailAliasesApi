using System.Net;
using System.Net.Mail;

namespace EmailAliasApi.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendPasswordResetEmail(string email, string resetToken, string resetUrl)
    {
        var smtpServer = _configuration["smtp:server"] ?? throw new InvalidOperationException("SMTP server not configured");
        var smtpUsername = _configuration["smtp:username"] ?? throw new InvalidOperationException("SMTP username not configured");
        var smtpPassword = _configuration["smtp:password"] ?? throw new InvalidOperationException("SMTP password not configured");
        var smtpPort = int.Parse(_configuration["smtp:sslport"] ?? "587");
        var enableSsl = bool.Parse(_configuration["smtp:enablessl"] ?? "false");

        var fromEmail = smtpUsername;
        var subject = "Password Reset Request";
        var body = $@"
Hello,

You have requested to reset your password. Please click the link below to reset your password:

{resetUrl}

This link will expire in 1 hour.

If you did not request a password reset, please ignore this email.

Best regards,
Email Aliases Manager
";

        try
        {
            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = enableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Password reset email sent to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
            throw;
        }
    }
}

