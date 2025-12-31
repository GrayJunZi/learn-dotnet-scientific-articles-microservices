using BuildingBlocks.Core.FluentValidation;
using Submission.Domain.Enums;

namespace Submission.Application.Features.AssignAuthor;

public record AssignAuthorCommand(int AuthorId, bool IsCorrespondingAuthor, HashSet<ContributeArea> ContributeAreas)
    : ArticleCommand
{
    public override ArticleActionType ActionType => ArticleActionType.AssignAuthor;
}

public class AssignAuthorCommandValidator : ArticleCommandValidator<AssignAuthorCommand>
{
    public AssignAuthorCommandValidator()
    {
        RuleFor(x => x.AuthorId).GreaterThan(0).WithMessageForInvalidId(nameof(AssignAuthorCommand.AuthorId));

        RuleFor(x => x.ContributeAreas)
            .WithMessageForNotEmpty(nameof(AssignAuthorCommand.ContributeAreas));
    }
}