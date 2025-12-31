using System.Text.Json.Serialization;
using BuildingBlocks.Core.FluentValidation;
using Submission.Domain.Enums;

namespace Submission.Application.Features.Shared;

public abstract record ArticleCommand<TActionType, TResponse> : IArticleAction<TActionType>, IRequest<TResponse>
    where TActionType : Enum
{
    [JsonIgnore] public int ArticleId { get; set; }

    public string? Comment { get; set; }

    [JsonIgnore] public abstract TActionType ActionType { get; }

    [JsonIgnore] public string Action => ActionType.ToString();

    [JsonIgnore] public DateTime CreatedOn => DateTime.UtcNow;

    [JsonIgnore] public int CreatedByUserId { get; set; }
}

public abstract record ArticleCommand : ArticleCommand<ArticleActionType, IdResponse>;

public abstract class ArticleCommandValidator<TFileActionCommand> : AbstractValidator<TFileActionCommand>
    where TFileActionCommand : IArticleAction
{
    public ArticleCommandValidator()
    {
        RuleFor(x => x.ArticleId).GreaterThan(0).WithMessageForInvalidId(nameof(ArticleCommand.ArticleId));
    }
}