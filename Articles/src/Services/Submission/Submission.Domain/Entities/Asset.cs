using Articles.Abstractions.Enums;
using BuildingBlocks.Domain.Entities;
using Submission.Domain.ValueObjects;

namespace Submission.Domain.Entities;

public partial class Asset : IEntity
{
    public int Id { get; init; }

    public AssetName Name { get; private set; }

    public AssetType Type { get; private set; }

    public int ArticleId { get; private set; }
    public Article Article { get; private set; }
    public ValueObjects.File File { get; set; }
}