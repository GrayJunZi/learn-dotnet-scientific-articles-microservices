using System.Text.Json.Serialization;
using BuildingBlocks.Core.FluentValidation;
using Submission.Domain.Enums;

namespace Submission.Application.Features.CreateArticle;

public record CreateArticleCommand(int JournalId, string Title, string Scope, ArticleType Type)
    : ArticleCommand
{
    [JsonIgnore] public DateTime CreatedOn => DateTime.UtcNow;
    [JsonIgnore] public int CreatedByUserId { get; set; }
    public override ArticleActionType ActionType => ArticleActionType.Create;
}

public class CreateArticleCommandValidator : ArticleCommandValidator<CreateArticleCommand>
{
    public CreateArticleCommandValidator()
    {
        RuleFor(x => x.Title)
            .WithMessageForNotEmpty(nameof(CreateArticleCommand.Title));

        RuleFor(x => x.Scope)
            .WithMessageForNotEmpty(nameof(CreateArticleCommand.Scope));

        RuleFor(x => x.JournalId)
            .GreaterThan(0)
            .WithMessageForInvalidId(nameof(CreateArticleCommand.JournalId));
    }
}