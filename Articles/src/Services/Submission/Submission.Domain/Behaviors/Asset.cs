using Submission.Domain.ValueObjects;

namespace Submission.Domain.Entities;

public partial class Asset
{
    private Asset()
    {
    }

    public string GenerateStorageFilePath(string fileName)
        => $"Articles/{ArticleId}/{Name}/{fileName}";
    
    internal static Asset Create(Article article, AssetTypeDefinition assetTypeDefinition)
    {
        return new Asset
        {
            ArticleId = article.Id,
            Article = article,
            Name = AssetName.FromAssetType(assetTypeDefinition),
            Type = assetTypeDefinition.Name,
        };
    }
}