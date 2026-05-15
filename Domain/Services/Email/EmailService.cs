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

    public async Task SendCompanyUnderReviewEmailAsync(
        string recipientName,
        string recipientEmail,
        string companyName)
    {
        var message = new EmailMessage
        {
            From = From,
            Subject = "Sua orquestra está afinando — cadastro recebido"
        };
        message.To.Add(recipientEmail);
        message.HtmlBody = BuildCompanyUnderReviewTemplate(recipientName, companyName);

        await _resend.EmailSendAsync(message);
    }

    public async Task SendCompanyApprovedEmailAsync(
        string recipientName,
        string recipientEmail,
        string fantasyName)
    {
        var message = new EmailMessage
        {
            From = From,
            Subject = "Seu painel Orchestra está liberado!"
        };
        message.To.Add(recipientEmail);
        message.HtmlBody = BuildCompanyApprovedTemplate(recipientName, fantasyName);

        await _resend.EmailSendAsync(message);
    }

    private static string Wrap(string preheader, string body) => $"""
        <!DOCTYPE html>
        <html lang="pt-BR">
        <head><meta charset="UTF-8" /><title>Orchestra</title></head>
        <body style="margin:0;padding:0;background:#f2eee3;font-family:'Inter',Helvetica,Arial,sans-serif;color:#1a2238;">
          <div style="display:none;font-size:1px;color:#f2eee3;line-height:1px;max-height:0;overflow:hidden;">{preheader}</div>
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f2eee3;">
            <tr><td align="center" style="padding:32px 16px;">
              <table width="600" cellpadding="0" cellspacing="0" style="max-width:600px;width:100%;background:#ffffff;border-radius:12px;border:1px solid #d4cfc4;">
                {body}
              </table>
            </td></tr>
          </table>
        </body></html>
        """;

    private static string BuildWelcomeTemplate(string name) => Wrap(
        $"Olá {name}, sua conta Orchestra foi criada com sucesso.",
        $"""
        <tr><td style="padding:40px;">
          <h1 style="margin:0 0 16px;font-family:'Instrument Serif',Georgia,serif;font-size:32px;font-weight:400;letter-spacing:-0.8px;color:#1a2238;">
            Olá, {name}!
          </h1>
          <p style="margin:0 0 14px;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:15px;line-height:1.6;color:#5a6478;">
            Sua conta na Orchestra foi criada com sucesso. Você já pode buscar
            serviços residenciais, comerciais e industriais na nossa plataforma.
          </p>
        </td></tr>
        """);

    private static string BuildCompanyApprovedTemplate(string name, string fantasyName)
    {
        var slug = fantasyName
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "e")
            .Trim('-');

        var dashboardUrl = $"https://app.orchestra.com.br/app/{slug}";

        return Wrap(
            $"Parabéns! O painel da {fantasyName} foi aprovado e já está disponível.",
            $"""
            <tr><td style="padding:40px;">
              <table width="100%" cellpadding="0" cellspacing="0">
                <tr><td align="center" style="padding-bottom:32px;">
                  <table cellpadding="0" cellspacing="0"><tr><td align="center"
                    style="width:64px;height:64px;background:#1a2238;border-radius:50%;color:#f2eee3;font-size:28px;line-height:64px;text-align:center;">
                    ✦
                  </td></tr></table>

                  <p style="margin:16px 0 4px;font-family:'JetBrains Mono',Consolas,monospace;font-size:10px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#8a8473;">
                    Painel liberado · {fantasyName}
                  </p>

                  <h1 style="margin:0 0 16px;font-family:'Instrument Serif',Georgia,serif;font-size:38px;font-weight:400;letter-spacing:-0.8px;color:#1a2238;line-height:1.1;">
                    Seu painel está pronto.
                  </h1>

                  <p style="max-width:420px;margin:0 auto;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:15px;line-height:1.5;color:#5a6478;">
                    Olá {name}, sua empresa <strong>{fantasyName}</strong> foi aprovada pela equipe Orchestra.
                    Seu painel já está disponível — acesse agora e comece a receber pedidos.
                  </p>
                </td></tr>

                <tr><td style="padding:20px;border:1px solid #d4cfc4;border-radius:12px;background:#ffffff;">
                  <p style="margin:0 0 16px;font-family:'JetBrains Mono',Consolas,monospace;font-size:10px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#8a8473;">
                    Progresso da análise
                  </p>

                  <table width="100%" cellpadding="0" cellspacing="0">
                    <tr><td style="padding-bottom:16px;">
                      <table cellpadding="0" cellspacing="0"><tr>
                        <td valign="top" style="width:24px;color:#2d7a3f;font-size:16px;font-weight:700;line-height:1.2;">✓</td>
                        <td valign="top">
                          <p style="margin:0;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:13px;font-weight:500;color:#1a2238;">
                            Cadastro recebido
                          </p>
                        </td>
                      </tr></table>
                    </td></tr>

                    <tr><td style="padding-bottom:16px;">
                      <table cellpadding="0" cellspacing="0"><tr>
                        <td valign="top" style="width:24px;color:#2d7a3f;font-size:16px;font-weight:700;line-height:1.2;">✓</td>
                        <td valign="top">
                          <p style="margin:0;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:13px;font-weight:500;color:#1a2238;">
                            Verificação de documentos
                          </p>
                        </td>
                      </tr></table>
                    </td></tr>

                    <tr><td>
                      <table cellpadding="0" cellspacing="0"><tr>
                        <td valign="top" style="width:24px;color:#2d7a3f;font-size:16px;font-weight:700;line-height:1.2;">✓</td>
                        <td valign="top">
                          <p style="margin:0;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:13px;font-weight:500;color:#1a2238;">
                            Painel liberado
                          </p>
                        </td>
                      </tr></table>
                    </td></tr>
                  </table>
                </td></tr>

                <tr><td style="padding-top:24px;text-align:center;">
                  <a href="{dashboardUrl}"
                     style="display:inline-block;padding:14px 32px;background:#1a2238;border-radius:8px;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:14px;font-weight:600;color:#f2eee3;text-decoration:none;letter-spacing:0.2px;">
                    Acessar painel
                  </a>

                  <p style="margin:16px 0 0;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:12px;color:#8a8473;">
                    Ou acesse diretamente: <a href="{dashboardUrl}" style="color:#1a52d7;text-decoration:none;">{dashboardUrl}</a>
                  </p>
                </td></tr>
              </table>
            </td></tr>
            """);
    }

    private static string BuildCompanyUnderReviewTemplate(string name, string companyName)
    {
        var receivedAt = DateTime.UtcNow;
        var dateLabel = receivedAt.ToString("yyyy-MM-dd");
        var timeLabel = receivedAt.ToString("HH:mm");
        var companyFragment = string.IsNullOrWhiteSpace(companyName) ? "" : $" da {companyName}";

        return Wrap(
            $"Recebemos os dados{companyFragment}. Validamos tudo em até 48h úteis.",
            $"""
            <tr><td style="padding:40px;">
              <table width="100%" cellpadding="0" cellspacing="0">
                <tr><td align="center" style="padding-bottom:32px;">
                  <table cellpadding="0" cellspacing="0"><tr><td align="center"
                    style="width:64px;height:64px;background:#1a2238;border-radius:50%;color:#f2eee3;font-size:28px;line-height:64px;text-align:center;">
                    ✦
                  </td></tr></table>

                  <p style="margin:16px 0 4px;font-family:'JetBrains Mono',Consolas,monospace;font-size:10px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#8a8473;">
                    Cadastro recebido · ORC-{dateLabel}
                  </p>

                  <h1 style="margin:0 0 16px;font-family:'Instrument Serif',Georgia,serif;font-size:38px;font-weight:400;letter-spacing:-0.8px;color:#1a2238;line-height:1.1;">
                    Sua orquestra está afinando.
                  </h1>

                  <p style="max-width:420px;margin:0 auto;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:15px;line-height:1.5;color:#5a6478;">
                    Olá {name}, recebemos os dados{companyFragment}. Em até 48h úteis nossa
                    equipe valida tudo e libera seu painel — você recebe um e-mail assim
                    que estiver pronto.
                  </p>
                </td></tr>

                <tr><td style="padding:20px;border:1px solid #d4cfc4;border-radius:12px;background:#ffffff;">
                  <p style="margin:0 0 16px;font-family:'JetBrains Mono',Consolas,monospace;font-size:10px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#8a8473;">
                    Progresso da análise
                  </p>

                  <table width="100%" cellpadding="0" cellspacing="0">
                    <tr><td style="padding-bottom:16px;">
                      <table cellpadding="0" cellspacing="0"><tr>
                        <td valign="top" style="width:24px;color:#2d7a3f;font-size:16px;font-weight:700;line-height:1.2;">✓</td>
                        <td valign="top">
                          <p style="margin:0;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:13px;font-weight:500;color:#1a2238;">
                            Cadastro recebido
                          </p>
                          <p style="margin:2px 0 0;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:12px;color:#8a8473;">
                            Hoje · {timeLabel}
                          </p>
                        </td>
                      </tr></table>
                    </td></tr>

                    <tr><td style="padding-bottom:16px;">
                      <table cellpadding="0" cellspacing="0"><tr>
                        <td valign="top" style="width:24px;color:#1a52d7;font-size:16px;font-weight:700;line-height:1.2;">◐</td>
                        <td valign="top">
                          <p style="margin:0;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:13px;font-weight:500;color:#1a2238;">
                            Verificação de documentos
                            <span style="display:inline-block;margin-left:6px;padding:2px 8px;background:#e6edfb;border-radius:999px;font-family:'JetBrains Mono',Consolas,monospace;font-size:10px;font-weight:500;letter-spacing:1px;text-transform:uppercase;color:#1a52d7;">
                              agora
                            </span>
                          </p>
                          <p style="margin:2px 0 0;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:12px;color:#8a8473;">
                            Em andamento · normalmente ~6h
                          </p>
                        </td>
                      </tr></table>
                    </td></tr>

                    <tr><td>
                      <table cellpadding="0" cellspacing="0"><tr>
                        <td valign="top" style="width:24px;color:#d4cfc4;font-size:16px;font-weight:700;line-height:1.2;">○</td>
                        <td valign="top">
                          <p style="margin:0;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:13px;font-weight:500;color:#8a8473;">
                            Painel liberado
                          </p>
                          <p style="margin:2px 0 0;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:12px;color:#8a8473;">
                            Você receberá um aviso por e-mail.
                          </p>
                        </td>
                      </tr></table>
                    </td></tr>
                  </table>
                </td></tr>

                <tr><td style="padding-top:24px;">
                  <p style="margin:0 0 12px;font-family:'JetBrains Mono',Consolas,monospace;font-size:10px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#8a8473;">
                    Enquanto isso
                  </p>

                  <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                      <td valign="top" width="50%" style="padding:16px;border:1px solid #d4cfc4;border-radius:12px;background:#ffffff;">
                        <p style="margin:0 0 8px;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:14px;font-weight:600;color:#1a2238;">
                          Manual do parceiro
                        </p>
                        <p style="margin:0;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:12px;color:#5a6478;">
                          15 min de leitura para tirar mais dos primeiros pedidos.
                        </p>
                      </td>
                      <td width="12"></td>
                      <td valign="top" width="50%" style="padding:16px;border:1px solid #d4cfc4;border-radius:12px;background:#ffffff;">
                        <p style="margin:0 0 8px;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:14px;font-weight:600;color:#1a2238;">
                          Baixe o app
                        </p>
                        <p style="margin:0;font-family:'Inter',Helvetica,Arial,sans-serif;font-size:12px;color:#5a6478;">
                          Receba pedidos no celular assim que abrirmos sua conta.
                        </p>
                      </td>
                    </tr>
                  </table>
                </td></tr>
              </table>
            </td></tr>
            """);
    }
}
