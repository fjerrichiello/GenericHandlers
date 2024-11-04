using Common.Messaging;
using FluentValidation.Results;

namespace Common.Helpers;

public static class ValidationExtensions
{
    public static ValidationFailedMessage ToValidationFailedMessage(this ValidationResult result, Type failedEventType)
    {
        return new ValidationFailedMessage(result.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key.ToSnakeCase(),
                g => g.Select(x => x.ErrorMessage).ToArray()
            ), failedEventType);
    }
}