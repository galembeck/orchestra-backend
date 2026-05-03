// EMAIL SENDING TEMPORARILY DISABLED — uncomment to re-enable.
#if false
using Domain.Constants;
using Resend;

namespace Domain.Services;

public class EmailService(IResend resend) : IEmailService
{
    private readonly IResend _resend = resend;

    private static string From =>
        $"{Constant.Settings.EmailServiceSettings.FromName} <{Constant.Settings.EmailServiceSettings.FromEmail}>";

    public async Task SendWelcomeEmailAsync(string recipientName, string recipientEmail)
    {
        var message = new EmailMessage
        {
            From = From,
            Subject = "Bem-vindo(a) à Orchestra!"
        };
        message.To.Add(recipientEmail);
        message.HtmlBody = BuildWelcomeTemplate(recipientName);

        await _resend.EmailSendAsync(message);
    }

    public async Task SendPasswordRecoveryEmailAsync(
        string recipientName,
        string recipientEmail,
        string token,
        DateTime expiresAt)
    {
        var message = new EmailMessage
        {
            From = From,
            Subject = "Código de recuperação de senha — Orchestra"
        };
        message.To.Add(recipientEmail);
        message.HtmlBody = BuildPasswordRecoveryTemplate(recipientName, token, expiresAt);

        await _resend.EmailSendAsync(message);
    }

    private static string Wrap(string preheader, string body) => $"""
        <!DOCTYPE html>
        <html lang="pt-BR">
        <head><meta charset="UTF-8" /><title>Orchestra</title></head>
        <body style="margin:0;padding:0;background:#f5f5f5;font-family:Helvetica,Arial,sans-serif;color:#222;">
          <div style="display:none;font-size:1px;color:#f5f5f5;line-height:1px;max-height:0;overflow:hidden;">{preheader}</div>
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f5f5f5;">
            <tr><td align="center" style="padding:24px 16px;">
              <table width="600" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;background:#fff;border-radius:8px;">
                {body}
              </table>
            </td></tr>
          </table>
        </body></html>
        """;

    private static string BuildWelcomeTemplate(string name) => Wrap(
        $"Olá {name}, sua conta Orchestra foi criada com sucesso.",
        $"""
        <tr><td style="padding:32px 40px;">
          <h1 style="margin:0 0 16px;font-size:22px;color:#222;">Olá, {name}!</h1>
          <p style="margin:0 0 14px;font-size:15px;line-height:1.6;color:#444;">
            Sua conta na Orchestra foi criada com sucesso. Você já pode buscar
            serviços residenciais, comerciais e industriais na nossa plataforma.
          </p>
        </td></tr>
        """);

    private static string BuildPasswordRecoveryTemplate(string name, string token, DateTime expiresAt) => Wrap(
        "Seu código de recuperação de senha Orchestra está disponível.",
        $"""
        <tr><td style="padding:32px 40px;">
          <h1 style="margin:0 0 16px;font-size:20px;color:#222;">Recuperação de senha</h1>
          <p style="margin:0 0 8px;font-size:15px;color:#222;">Olá, <strong>{name}</strong>!</p>
          <p style="margin:0 0 20px;font-size:14px;line-height:1.6;color:#555;">
            Use o código abaixo para redefinir sua senha. Ele expira em
            <strong>{Constant.Settings.AuthSettings.RecoveryPasswordExpiration} minutos</strong>.
          </p>
          <div style="background:#fafafa;border:2px dashed #888;border-radius:8px;padding:24px;text-align:center;margin-bottom:20px;">
            <p style="margin:0 0 6px;font-size:11px;color:#888;letter-spacing:3px;text-transform:uppercase;">Seu código</p>
            <p style="margin:0;font-size:32px;font-weight:700;letter-spacing:10px;color:#222;">{token}</p>
          </div>
          <p style="margin:0;font-size:13px;color:#888;">
            Se você não solicitou esta recuperação, ignore este e-mail.
          </p>
        </td></tr>
        """);
}
#endif
