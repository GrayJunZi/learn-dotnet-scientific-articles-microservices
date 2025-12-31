using BuildingBlocks.Domain.ValueObjects;
using Submission.Domain.Entities;

namespace Submission.Domain.ValueObjects;

public class AssetName : StringValueObject
{
    private AssetName(string value) => Value = value;

    public static AssetName FromAssetType(AssetTypeDefinition assetTypeDefinition)
        => new AssetName(assetTypeDefinition.Name.ToString());
}