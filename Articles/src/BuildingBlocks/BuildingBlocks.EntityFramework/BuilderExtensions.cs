using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BuildingBlocks.EntityFramework;

public static class BuilderExtensions
{
    public static PropertyBuilder<TEnum> HasEnumConversion<TEnum>(this PropertyBuilder<TEnum> builder)
        where TEnum : Enum
    {
        return builder.HasConversion(
            x => x.ToString(),
            x => (TEnum)Enum.Parse(typeof(TEnum), x)
        );
    }

    public static PropertyBuilder<T> HasJsonCollectionConversion<T>(this PropertyBuilder<T> builder)
    {
        return builder.HasConversion(BuildJsonListConvertor<T>());
    }

    public static ValueConverter<TCollection, string> BuildJsonListConvertor<TCollection>()
    {
        Func<TCollection, string> serializeFunc = x => JsonSerializer.Serialize(x);
        Func<string, TCollection> deserializeFunc = x => JsonSerializer.Deserialize<TCollection>(x ?? "{}");

        return new ValueConverter<TCollection, string>(
            x => serializeFunc(x),
            x => deserializeFunc(x)
        );
    }
}