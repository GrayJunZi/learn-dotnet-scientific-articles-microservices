using BuildingBlocks.Domain.Entities;
using Submission.Domain.ValueObjects;

namespace Submission.Domain.Entities;

public class Person : IEntity
{
    public int Id { get; init; }
    public required string Name { get; init; }

    public string? Title { get; set; }

    public required EmailAddress EmailAddress { get; init; }
    public required string Affiliation { get; init; }

    public int? UserId { get; init; }

    public string TypeDiscriminator { get; init; }

    public IReadOnlyList<ArticleActor> Actors { get; private set; } = new List<ArticleActor>();
}