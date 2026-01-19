using Auth.Domain.Users;
using Auth.Domain.Users.Events;
using BuildingBlocks.AspNetCore.Extensions;
using EmailService.Contracts;
using EmailService.Smtp;
using FastEndpoints;
using Flurl;
using Microsoft.Extensions.Options;

namespace Auth.API.Features.CreateUser;

public class SendConfirmationEmailOnUserCreatedHandler(
    IHttpContextAccessor httpContextAccessor,
    IEmailService emailService,
    IOptions<EmailOptions> emailOptions)
    : IEventHandler<UserCreated>
{
    public async Task HandleAsync(UserCreated eventModel, CancellationToken ct)
    {
        var url = httpContextAccessor.HttpContext?.Request.BaseUrl()
            .AppendPathSegment("password")
            .SetQueryParams(new { eventModel.ResetToken });

        var emailMessage = buildConfirmationEmail(eventModel.User, url, emailOptions.Value.EmailFromAddress);
        await emailService.SendEmailAsync(emailMessage, ct);
    }

    private EmailMessage buildConfirmationEmail(User user, string resetLink, string fromEmailAddress)
    {
        const string confirmationEmail =
            "Dear {0},<br/>An account has been created for you.<br/>Please set your password using the following URL: <br/>{1}";
        return new EmailMessage(
            "Your Account Has Been Created - Set Your Password",
            new Content(ContentType.Html, string.Format(confirmationEmail, user.Name, resetLink)),
            new EmailAddress("Articles", fromEmailAddress),
            [new EmailAddress(user.Name, user.Email)]);
    }
}