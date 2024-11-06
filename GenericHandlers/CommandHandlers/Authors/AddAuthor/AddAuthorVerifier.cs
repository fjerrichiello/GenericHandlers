using Common.Messaging;
using Common.Verifiers;
using FluentValidation;
using GenericHandlers.Commands.Authors;

namespace GenericHandlers.CommandHandlers.Authors.AddAuthor;

public class AddAuthorVerifier : AuthorizedMessageVerifier<AddAuthorCommand, CommandMetadata, AddAuthorUnverifiedData>
{
    protected override void AuthorizationRules()
    {
        RuleFor(parameters => parameters.DataFactoryResult.FirstName)
            .NotEmpty();
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