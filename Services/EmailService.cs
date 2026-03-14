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
        var body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
</head>
<body style=""margin:0;padding:0;background-color:#fef7f1;font-family:Georgia,serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#fef7f1;padding:40px 0;"">
    <tr>
      <td align=""center"">
        <table width=""520"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 2px 12px rgba(69,9,32,0.08);"">

          <!-- Header -->
          <tr>
            <td style=""background-color:#450920;padding:32px 40px;text-align:center;"">
              <p style=""margin:0;color:#f9dbbd;font-size:11px;letter-spacing:3px;text-transform:uppercase;"">Email Aliases</p>
              <h1 style=""margin:8px 0 0;color:#fef7f1;font-size:22px;font-weight:400;letter-spacing:1px;"">Password Reset</h1>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style=""padding:40px 40px 32px;"">
              <p style=""margin:0 0 20px;color:#450920;font-size:15px;line-height:1.7;"">Hello,</p>
              <p style=""margin:0 0 28px;color:#5a3040;font-size:15px;line-height:1.7;"">
                We received a request to reset the password for your account. Click the button below to choose a new password.
              </p>

              <!-- Button -->
              <table cellpadding=""0"" cellspacing=""0"" style=""margin:0 auto 28px;"">
                <tr>
                  <td style=""background-color:#da627d;border-radius:8px;"">
                    <a href=""{resetUrl}"" style=""display:inline-block;padding:14px 32px;color:#ffffff;font-size:15px;text-decoration:none;letter-spacing:0.5px;"">
                      Reset Password
                    </a>
                  </td>
                </tr>
              </table>

              <p style=""margin:0 0 8px;color:#7a4a5a;font-size:13px;line-height:1.6;"">
                Or copy this link into your browser:
              </p>
              <p style=""margin:0 0 28px;word-break:break-all;"">
                <a href=""{resetUrl}"" style=""color:#a53860;font-size:13px;"">{resetUrl}</a>
              </p>

              <hr style=""border:none;border-top:1px solid #f9dbbd;margin:0 0 24px;"" />

              <p style=""margin:0;color:#9a6a7a;font-size:13px;line-height:1.6;"">
                This link expires in <strong>1 hour</strong>. If you did not request a password reset, you can safely ignore this email.
              </p>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style=""background-color:#fef0e8;padding:20px 40px;text-align:center;"">
              <p style=""margin:0;color:#b08090;font-size:12px;letter-spacing:1px;"">EMAIL ALIASES MANAGER</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";

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
                IsBodyHtml = true
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

