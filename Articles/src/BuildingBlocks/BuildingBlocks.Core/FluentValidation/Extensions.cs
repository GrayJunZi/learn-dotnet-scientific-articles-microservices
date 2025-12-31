using FluentValidation;

namespace BuildingBlocks.Core.FluentValidation;

public static class Extensions
{
    public static IRuleBuilderOptions<T, TProperty> WithMessageForInvalidId<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> ruleBuilder, string propertyName)
        => ruleBuilder.WithMessage(x => ValidationMessages.InvalidId.FormatWith(propertyName));

    public static IRuleBuilderOptions<T, TProperty> WithMessageForNotEmpty<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder, string propertyName)
        => ruleBuilder.NotEmpty().WithMessage(x => ValidationMessages.NullOrEmptyValue.FormatWith(propertyName));

    public static IRuleBuilderOptions<T, string?> WithMessageMaximumLength<T>(
        this IRuleBuilder<T, string?> ruleBuilder, int maximumLength, string propertyName)
        => ruleBuilder.MaximumLength(maximumLength)
            .WithMessage(x => ValidationMessages.MaxLengthExceeded.FormatWith(propertyName, maximumLength));

    public static IRuleBuilderOptions<T, TProperty> WithMessageForNotNull<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder)
        => ruleBuilder.NotNull()
            .WithMessage(x => ValidationMessages.NullOrEmptyValue.FormatWith(typeof(TProperty).Name));
}

public static class ValidationMessages
{
    public const string InvalidId = "The {0} should be greater than zero.";
    public const string MaxLengthExceeded = "{0} must not exceed {1} characters.";
    public const string NullOrEmptyValue = "{0} is required.";
}