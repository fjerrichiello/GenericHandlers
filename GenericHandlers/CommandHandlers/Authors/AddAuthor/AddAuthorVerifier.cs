using Common.Messaging;
using Common.Verifiers;
using Common.Verifiers.Rules;
using FluentValidation;
using GenericHandlers.Commands.Authors;

namespace GenericHandlers.CommandHandlers.Authors.AddAuthor;

public class AddAuthorVerifier : AuthorizedMessageVerifier<AddAuthorCommand, CommandMetadata, AddAuthorUnverifiedData>
{
    protected override void AuthorizationRules()
    {
        RuleFor(parameters => parameters.DataFactoryResult.FirstName)
            .NotEmpty();

        RuleFor(parameters => parameters)
            .Must(VerificationRules.HasFirstName);
    }

    protected override void ValidationRules()
    {
        RuleFor(parameters => parameters.DataFactoryResult.Author)
            .Null();

        RuleFor(parameters => parameters.DataFactoryResult.FirstName)
            .NotEmpty();

        RuleFor(parameters => parameters.DataFactoryResult.LastName)
            .NotEmpty();
    }
}