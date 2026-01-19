using System.Security.Claims;
using Auth.Application;
using Auth.Domain.Users;
using BuildingBlocks.AspNetCore.Extensions;
using BuildingBlocks.Exceptions;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Features.Login;

[AllowAnonymous]
[HttpPost("login")]
public class LoginEndpoint(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    TokenFactory tokenFactory) : Endpoint<LoginCommand, LoginResponse>
{
    public override async Task HandleAsync(LoginCommand req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        if (user is null)
            throw new BadRequestException($"User {req.Email} not found");

        var result = await signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!result.Succeeded)
            throw new BadRequestException($"Invalid credentials for user {req.Email}");

        var userRoles = await userManager.GetRolesAsync(user);

        var jwtToken =
            tokenFactory.GenerateAccessToken(user.Id.ToString(), user.Name, req.Email, userRoles, Array.Empty<Claim>());
        var refreshToken = tokenFactory.GenerateRefreshToken(HttpContext.GetClientIpAddress());

        user.AddRefreshToken(refreshToken);
        await userManager.UpdateAsync(user);

        await Send.OkAsync(new LoginResponse(req.Email, jwtToken, refreshToken.Token));
    }
}