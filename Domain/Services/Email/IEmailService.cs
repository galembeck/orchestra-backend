// EMAIL SENDING TEMPORARILY DISABLED — uncomment to re-enable.
#if false
namespace Domain.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string recipientName, string recipientEmail);

    Task SendPasswordRecoveryEmailAsync(
        string recipientName,
        string recipientEmail,
        string token,
        DateTime expiresAt);
}
#endif
