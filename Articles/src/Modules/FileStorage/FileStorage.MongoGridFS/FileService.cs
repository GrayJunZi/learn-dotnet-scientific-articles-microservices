using FileStorage.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace FileStorage.MongoGridFS;

public class FileService : IFileService
{
    private readonly GridFSBucket _gridFsBucket;
    private readonly MongoGridFSFileStorageOptions _options;

    private const string FilePathMetadataKey = "filePath";
    private const string ContentTypeMetadataKey = "contentType";
    private const string DefaultContentType = "application/octet-stream";

    public FileService(GridFSBucket gridFSBucket, IOptions<MongoGridFSFileStorageOptions> options)
        => (_gridFsBucket, _options) = (gridFSBucket, options.Value);

    public async Task<UploadResponse> UploadFileAsync(string filePath, IFormFile file, bool overwrite = false,
        Dictionary<string, string>? tags = null)
    {
        if (file.Length > _options.FileSizeLimitInBytes)
            throw new InvalidOperationException(
                $"File exceeds maximum allowed size of {_options.FileSizeLimitInMB} MB.");

        var metadata = new BsonDocument(tags ?? new())
        {
            { FilePathMetadataKey, filePath },
            { ContentTypeMetadataKey, file.ContentType },
        };

        var uploadOptions = new GridFSUploadOptions
        {
            Metadata = metadata,
            ChunkSizeBytes = _options.ChunkSizeBytes,
        };

        using var stream = file.OpenReadStream();
        var fileId = await _gridFsBucket.UploadFromStreamAsync(file.FileName, stream, uploadOptions);

        return new UploadResponse(
            FilePath: filePath,
            FileName: file.FileName,
            FileSize: file.Length,
            FileId: fileId.ToString());
    }

    public async Task<(Stream FileStream, string ContentType)> DownloadFileAsync(string fileId)
    {
        if (!ObjectId.TryParse(fileId, out ObjectId objectId))
            throw new FileNotFoundException($"Invalid file id '{fileId}'.");

        var fileInfo = await _gridFsBucket
            .Find(Builders<GridFSFileInfo>.Filter.Eq("_id", fileId))
            .FirstOrDefaultAsync();

        if (fileInfo is null)
            throw new FileNotFoundException($"File not found with id: '{fileId}'.");

        var stream = await _gridFsBucket.OpenDownloadStreamAsync(fileId);
        var contentType = fileInfo.Metadata
            .GetValue(ContentTypeMetadataKey, DefaultContentType)
            ?.AsString ?? DefaultContentType;

        return (stream, contentType);
    }

    public async Task<bool> TryDeleteFileAsync(string fileId)
    {
        if (!ObjectId.TryParse(fileId, out ObjectId objectId))
            return false;

        try
        {
            await _gridFsBucket.DeleteAsync(objectId);
            return true;
        }
        catch (GridFSFileNotFoundException)
        {
            return false;
        }
    }
}