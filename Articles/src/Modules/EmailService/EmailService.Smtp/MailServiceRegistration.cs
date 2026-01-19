using BuildingBlocks.Core;
using EmailService.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmailService.Smtp;

public static class MailServiceRegistration
{
    public static IServiceCollection AddSmtpEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAndValidateOptions<EmailOptions>(configuration);
        services.AddSingleton<IEmailService, SmtpEmailService>();
        return services;    
    }
}