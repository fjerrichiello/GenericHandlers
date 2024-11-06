using FluentValidation;
using FluentValidation.Results;

namespace Common.Structured.Authorization;

public abstract class Authorizer<TParameters> : AbstractValidator<TParameters>,
    IAuthorizer<TParameters>
{
    public AuthorizationResult Authorize(TParameters parameters)
    {
        var result = Validate(parameters);

        return result.IsValid
            ? new AuthorizationResult()
            : new AuthorizationResult(
                "Not authorized for action");
    }
}