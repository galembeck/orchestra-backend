namespace Domain.Utils.Constants;

public sealed record EmailServiceSettings
{
    public string ApiToken { get; init; } = string.Empty;
    public string FromEmail { get; init; } = string.Empty;
    public string FromName { get; init; } = string.Empty;
}
