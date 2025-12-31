using BuildingBlocks.Core;
using FileStorage.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace FileStorage.MongoGridFS;

public static class FileStorageRegistration
{
    public static IServiceCollection AddMongoFileStorage(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAndValidateOptions<MongoGridFSFileStorageOptions>(configuration);

        var options = configuration.GetSectionByTypeName<MongoGridFSFileStorageOptions>();

        services.AddSingleton<IMongoClient>(sp =>
            new MongoClient(configuration.GetConnectionStringOrThrow(options.ConnectionStringName)));

        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.DatabaseName);
        });

        services.AddSingleton(sp =>
        {
            var db = sp.GetRequiredService<IMongoDatabase>();
            return new GridFSBucket(db, new GridFSBucketOptions
            {
                BucketName = options.BucketName,
                ChunkSizeBytes = options.ChunkSizeBytes,
                WriteConcern = WriteConcern.WMajority,
                ReadPreference = ReadPreference.Primary,
            });
        });

        services.AddSingleton<IFileService, FileService>();

        return services;
    }
}