using Common.Structured.Validation;
using FluentValidation;

namespace GenericHandlers.Structured.CommandHandlers.Authors.AddAuthor;

public class AddAuthorValidator
    : Validator<AddAuthorData>
{
    public AddAuthorValidator()
    {
        RuleFor(data => data.Author)
            .Null();

        RuleFor(data => data.FirstName)
            .NotEmpty();

        RuleFor(data => data.LastName)
            .NotEmpty();
    }
}