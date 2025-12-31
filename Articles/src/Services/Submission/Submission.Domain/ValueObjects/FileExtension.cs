using Articles.Abstractions.Enums;
using BuildingBlocks.Core;
using BuildingBlocks.Domain.ValueObjects;

namespace Submission.Domain.ValueObjects;

public class FileExtension : StringValueObject
{
    private FileExtension(string value) => Value = value;

    public static FileExtension FromFileExtension(string fileName, AssetType assetType)
    {
        var extension = Path.GetExtension(fileName).Remove(0, 1);
        Guard.ThrowIfNullOrWhiteSpace(extension);

        return new FileExtension(extension);
    }
}