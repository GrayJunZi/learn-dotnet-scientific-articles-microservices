using Articles.Abstractions.Enums;
using BuildingBlocks.Domain.Entities;

namespace Submission.Domain.Entities;

public partial class Article : Entity
{
    public required string Title { get; set; }
    public required string Scope { get; set; }
    public required ArticleType Type { get; set; }
    public ArticleStage Stage { get; internal set; }
    public int JournalId { get; set; }
    public required Journal Journal { get; init; }

    public IReadOnlyList<ArticleActor> Actors { get; private set; } = new List<ArticleActor>();

    private readonly List<Asset> _assets = new();
    public IReadOnlyList<Asset> Assets => _assets.AsReadOnly();
}