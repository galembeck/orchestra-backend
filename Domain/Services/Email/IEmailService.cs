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
