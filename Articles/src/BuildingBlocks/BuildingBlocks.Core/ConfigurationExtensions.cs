using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddAndValidateOptions<TOptions>(this IServiceCollection services,
        IConfiguration configuration)
        where TOptions : class
    {
        var section = configuration.GetSection(typeof(TOptions).Name);
        if (!section.Exists())
            throw new InvalidOperationException($"Configuration section {section.Key} does not exist.");

        services
            .AddOptions<TOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static T GetSectionByTypeName<T>(this IConfiguration configuration)
    {
        var sectionName = typeof(T).Name;
        var section = configuration.GetSection(sectionName).Get<T>();

        return Guard.AgainstNull(section, sectionName);
    }

    public static string GetConnectionStringOrThrow(this IConfiguration configuration, string name)
    {
        var value = configuration.GetConnectionString(name);
        if (string.IsNullOrEmpty(value))
            throw new InvalidOperationException($"Connection string '{name}' does not exist or empty.");

        return value;
    }
}