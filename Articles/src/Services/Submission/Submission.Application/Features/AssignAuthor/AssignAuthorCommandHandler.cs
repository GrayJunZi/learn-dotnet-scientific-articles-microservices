using BuildingBlocks.EntityFramework;

namespace Submission.Application.Features.AssignAuthor;

public class AssignAuthorCommandHandler(ArticleRepository articleRepository)
    : IRequestHandler<AssignAuthorCommand, IdResponse>
{
    public async Task<IdResponse> Handle(AssignAuthorCommand command, CancellationToken cancellationToken)
    {
        var article = await articleRepository.FindByIdOrThrowAsync(command.ArticleId);
        var author = await articleRepository.Context.Authors.FindByIdOrThrowAsync(command.AuthorId);

        article.AssignAuthor(author, command.ContributeAreas, command.IsCorrespondingAuthor);

        await articleRepository.SaveChangesAsync();

        return new IdResponse(article.Id);
    }
}