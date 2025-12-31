using Articles.Abstractions;
using Articles.Abstractions.Enums;
using Articles.Security;
using MediatR;
using Submission.Application.Features.CreateAndAssignAuthor;

namespace Submission.API.Endpoints;

public static class CreateAndAssignAuthorEndpoint
{
    public static void Map(this IEndpointRouteBuilder app)
    {
        app.MapPut("api/articles/{articleId:int}/authors/{authorId:int}",
                async (int articleId, CreateAndAssignAuthorCommand command, ISender sender) =>
                {
                    var response = await sender.Send(command with { ArticleId = articleId });
                    return Results.Ok(response);
                })
            .RequireRoleAuthorization(Role.CORAUT)
            .WithName("AssignAuthor")
            .WithTags("Articles")
            .Produces<IdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}