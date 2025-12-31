using BuildingBlocks.Core;

namespace Submission.Domain.ValueObjects;

public class FileExtensions
{
    public IReadOnlyList<string> Extensions { get; init; }

    public bool IsValidExtension(string extension)
        => Extensions.IsEmpty() || Extensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
}