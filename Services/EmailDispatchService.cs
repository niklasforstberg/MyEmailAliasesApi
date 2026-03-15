using System.Threading.Channels;

namespace EmailAliasApi.Services;

public record EmailJob(string To, string ResetToken, string ResetUrl);

public class EmailDispatchService : BackgroundService
{
    private readonly Channel<EmailJob> _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailDispatchService> _logger;

    public EmailDispatchService(Channel<EmailJob> channel, IServiceScopeFactory scopeFactory, ILogger<EmailDispatchService> logger)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
                await emailService.SendPasswordResetEmail(job.To, job.ResetToken, job.ResetUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", job.To);
            }
        }
    }
}
