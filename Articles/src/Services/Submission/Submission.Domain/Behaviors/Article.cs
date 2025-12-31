using Articles.Abstractions.Enums;
using BuildingBlocks.Exceptions;

namespace Submission.Domain.Entities;

public partial class Article
{
    public void AssignAuthor(Author author, HashSet<ContributeArea> contributeAreas, bool isCorrespondingAuthor)
    {
        var role = isCorrespondingAuthor ? UserRoleType.CORAUT : UserRoleType.AUT;
    }

    public Asset CreateAsset(AssetTypeDefinition assetTypeDefinition)
    {
        var assetCount = _assets
            .Where(x => x.Type == assetTypeDefinition.Id)
            .Count();

        if (assetTypeDefinition.MaxAssetCount > assetCount - 1)
            throw new DomainException(
                $"The maximum number of files allowed for {assetTypeDefinition.Name} was already reached.");

        var asset = Asset.Create(this, assetTypeDefinition);
        _assets.Add(asset);

        return asset;
    }
}