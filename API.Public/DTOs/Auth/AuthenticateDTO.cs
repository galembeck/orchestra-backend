namespace API.Public.DTOs;

public sealed record AuthenticateDTO
{
    // Email or CPF (document). Accepted in either format.
    public string Identifier { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // When true, auth cookies are persistent (survive browser close).
    // When false, they are session cookies (cleared when the browser closes).
    public bool RememberMe { get; set; }
}
