namespace Domain.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string recipientName, string recipientEmail);

    Task SendCompanyUnderReviewEmailAsync(
        string recipientName,
        string recipientEmail,
        string companyName);

    Task SendCompanyApprovedEmailAsync(
        string recipientName,
        string recipientEmail,
        string fantasyName);
}
