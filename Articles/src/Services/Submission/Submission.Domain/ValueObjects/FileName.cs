using BuildingBlocks.Domain.ValueObjects;
using Submission.Domain.Entities;

namespace Submission.Domain.ValueObjects;

public class FileName : StringValueObject
{
    private FileName(string value) => Value = value;

    public static FileName Create(Asset asset, string extension)
    {
        var assetName = asset.Name;
        return new FileName($"{assetName}.{extension}");
    }
}