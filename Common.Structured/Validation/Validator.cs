using Common.Structured.Authorization;
using FluentValidation;
using FluentValidation.Results;

namespace Common.Structured.Validation;

public abstract class Validator<TParameters> : AbstractValidator<TParameters>,
    IValidator<TParameters>
{
    public AuthorizationResult Authorize(TParameters parameters)
    {
        var result = Validate(parameters);

        return result.IsValid
            ? new AuthorizationResult()
            : new AuthorizationResult(
                "Not authorized for action");
    }

    ValidationResult IValidator<TParameters>.Validate(TParameters data)
    {
        return base.Validate(data);
    }
}