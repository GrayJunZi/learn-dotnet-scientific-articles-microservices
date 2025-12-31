using BuildingBlocks.Core.Constraints;
using BuildingBlocks.Core.FluentValidation;
using Submission.Domain.Enums;

namespace Submission.Application.Features.CreateAndAssignAuthor;

public record CreateAndAssignAuthorCommand(
    int? UserId,
    string? Name,
    string? EmailAddress,
    string? Title,
    string? Affiliation,
    HashSet<ContributeArea>? ContributeAreas,
    bool IsCorresponding)
    : ArticleCommand
{
    public override ArticleActionType ActionType => ArticleActionType.AssignAuthor;
}

public class CreateAndAssignAuthorCommandValidator : ArticleCommandValidator<CreateAndAssignAuthorCommand>
{
    public CreateAndAssignAuthorCommandValidator()
    {
        When(x => x.UserId == null, () =>
        {
            RuleFor(x => x.EmailAddress)
                .WithMessageForNotEmpty(nameof(CreateAndAssignAuthorCommand.EmailAddress))
                .WithMessageMaximumLength(MaxLength.C64, nameof(CreateAndAssignAuthorCommand.EmailAddress));

            RuleFor(x => x.Name)
                .WithMessageForNotEmpty(nameof(CreateAndAssignAuthorCommand.Name))
                .WithMessageMaximumLength(MaxLength.C64, nameof(CreateAndAssignAuthorCommand.Name));

            RuleFor(x => x.Affiliation)
                .WithMessageForNotEmpty(nameof(CreateAndAssignAuthorCommand.Affiliation))
                .WithMessageMaximumLength(MaxLength.C64, nameof(CreateAndAssignAuthorCommand.Affiliation));
        });

        RuleFor(x => x.ContributeAreas)
            .WithMessageForNotEmpty(nameof(CreateAndAssignAuthorCommand.ContributeAreas));
    }
}