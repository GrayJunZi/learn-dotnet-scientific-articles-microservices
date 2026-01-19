using Auth.Domain.Users.Enums;
using BuildingBlocks.Core;
using BuildingBlocks.Domain.ValueObjects;

namespace Auth.Domain.Users.ValueObjects;

public class HonorificTitle : StringValueObject
{
    private HonorificTitle(string value) => Value = value;

    public static HonorificTitle FromString(string honorific)
    {
        Guard.ThrowIfNullOrWhiteSpace(honorific);
        return new HonorificTitle(honorific.Trim());
    }

    public static HonorificTitle? FromEnum(Honorific? honorific)
    {
        if (honorific.HasValue)
            return new HonorificTitle(honorific.ToString());
        
        return null;
    }
}