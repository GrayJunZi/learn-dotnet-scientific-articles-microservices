namespace BuildingBlocks.Domain.Entities;

public abstract class EnumEntity<TEnum> : Entity<TEnum>
    where TEnum : struct, Enum
{
    public TEnum Name { get; init; }
}