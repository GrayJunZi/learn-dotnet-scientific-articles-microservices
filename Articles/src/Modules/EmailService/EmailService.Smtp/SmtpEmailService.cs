using EmailService.Contracts;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;

namespace EmailService.Smtp;

public class SmtpEmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;

    public SmtpEmailService(IOptions<EmailOptions> emailOptions)
    {
        _emailOptions = emailOptions.Value;
    }

    public async Task<bool> SendEmailAsync(EmailMessage emailMessage, CancellationToken cancellationToken)
    {
        var message = emailMessage.ToMailKitMessage();

        using var smtpClient = new SmtpClient();

        try
        {
            await smtpClient.ConnectAsync(
                _emailOptions.Smtp.Host,
                _emailOptions.Smtp.Port,
                _emailOptions.Smtp.UseSSL,
                cancellationToken
            );

            await smtpClient.AuthenticateAsync(
                _emailOptions.Smtp.Username,
                _emailOptions.Smtp.Password,
                cancellationToken
            );
            await smtpClient.SendAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }
}