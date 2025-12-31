using BuildingBlocks.EntityFramework;

namespace Submission.Application.Features.CreateAndAssignAuthor;

public class CreateAndAssignAuthorCommandHandler(ArticleRepository articleRepository)
    : IRequestHandler<CreateAndAssignAuthorCommand, IdResponse>
{
    public async Task<IdResponse> Handle(CreateAndAssignAuthorCommand command, CancellationToken cancellationToken)
    {
        var article = await articleRepository.FindByIdOrThrowAsync(command.ArticleId);

        Author? author = null;
        if (command.UserId is null)
            author = Author.Create(command.EmailAddress, command.Name, command.Title, command.Affiliation);
        else
            author = null;

        article.AssignAuthor(author, command.ContributeAreas, command.IsCorresponding);

        await articleRepository.SaveChangesAsync();

        return new IdResponse(article.Id);
    }
}