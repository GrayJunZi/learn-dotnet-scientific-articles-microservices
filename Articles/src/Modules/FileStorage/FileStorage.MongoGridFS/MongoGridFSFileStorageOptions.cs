using System.ComponentModel.DataAnnotations;

namespace FileStorage.MongoGridFS;

public class MongoGridFSFileStorageOptions
{
    [Required]
    public string ConnectionStringName { get; init; }
    [Required]
    public string DatabaseName { get; init; }
    [Required]
    public string BucketName { get; init; } = "files";
    public int ChunkSizeBytes { get; init; } = 1048576;
    public long FileSizeLimitInMB { get; init; } = 50;
    public long FileSizeLimitInBytes => FileSizeLimitInMB * 1024 * 1024;
}