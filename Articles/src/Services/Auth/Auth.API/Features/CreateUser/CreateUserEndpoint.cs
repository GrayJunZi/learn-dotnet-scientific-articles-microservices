using Articles.Abstractions.Enums;
using Auth.Domain.Users;
using Auth.Domain.Users.Events;
using BuildingBlocks.Exceptions;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Features.CreateUser;

[Authorize(Roles = Role.USERADMIN)]
[HttpPost("users")]
public class CreateUserEndpoint(UserManager<User> userManager) : Endpoint<CreateUserCommand, CreateUserResponse>
{
    public override async Task HandleAsync(CreateUserCommand req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        if (user is not null)
            throw new BadRequestException($"User with email {req.Email} alreay exits.");

        user = Domain.Users.User.Create(req);

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new BadRequestException($"Unable to create user {req.Email} with {errorMessages}");
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

        await PublishAsync(new UserCreated(user, resetToken));

        Response = new CreateUserResponse(req.Email, user.Id, resetToken);

        //await SendAsync(new CreateUserResponse(req.Email, user.Id, resetToken));
    }
}