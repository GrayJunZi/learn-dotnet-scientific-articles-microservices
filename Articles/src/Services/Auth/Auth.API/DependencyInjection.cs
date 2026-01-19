using System.Security.Claims;
using Auth.Domain.Role;
using Auth.Domain.Users;
using Auth.Persistence;
using BuildingBlocks.Core;
using EmailService.Smtp;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace Auth.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAndValidateOptions<EmailOptions>(configuration);
        
        services
            .AddFastEndpoints()
            .AddSwaggerDocument()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddJwtIdentity(configuration);

        services.AddSmtpEmailService(configuration);
        return services;
    }

    public static IServiceCollection AddJwtIdentity(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddIdentityCore<User>(options =>
        {
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
        }).AddRoles<Role>()
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddSignInManager<SignInManager<User>>()
        .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
        });
        return services;
    }

}