using FileStorage.MongoGridFS;

namespace Submission.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddMemoryCache()
            .AddOpenApi()
            .AddMongoFileStorage(configuration);
    }
} 