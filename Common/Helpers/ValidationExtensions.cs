using FluentValidation.Results;

namespace Common.Helpers;

public static class ValidationExtensions
{
    public static IDictionary<string, string[]> ToDictionarySnakeCase(this ValidationResult result)
    {
        return result.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key.ToSnakeCase(),
                g => g.Select(x => x.ErrorMessage).ToArray()
            );
    }
}