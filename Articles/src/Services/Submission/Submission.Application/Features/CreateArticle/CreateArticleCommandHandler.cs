using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Submission.Application.Features.CreateArticle;

public class CreateArticleCommandHandler(Repository<Journal> journalRepository)
    : IRequestHandler<CreateArticleCommand, IdResponse>
{
    public async Task<IdResponse> Handle(CreateArticleCommand command, CancellationToken cancellationToken)
    {
        var journal = await journalRepository.FindByIdOrThrowAsync(command.JournalId);

        var article = journal.CreateArticle(command.Title, command.Type, command.Scope);

        await assignCurrentUserAsAuthor(article, command);
        
        await journalRepository.SaveChangesAsync(cancellationToken);

        return new IdResponse(article.Id);
    }

    private async Task assignCurrentUserAsAuthor(Article article, CreateArticleCommand command)
    {
        var author = await journalRepository.Context.Authors
            .SingleOrDefaultAsync(x => x.UserId == command.CreatedByUserId);

        if (author is not null)
            article.AssignAuthor(author, [ContributeArea.OriginalDraft], true);
    }
}