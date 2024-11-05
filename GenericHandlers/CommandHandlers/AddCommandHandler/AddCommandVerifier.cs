using Common.Messaging;
using Common.Verifiers;
using FluentValidation;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.AddCommandHandler;

public class AddCommandVerifier : AuthorizedMessageVerifier<AddCommand, CommandMetadata, AddCommandUnverifiedData>
{
    protected override void AuthorizationRules()
    {
        RuleFor(x => x.MessageContainer.Message)
            .NotEmpty();
    }

    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .LessThan(0);

        RuleFor(x => x.DataFactoryResult.TestValue3)
            .NotEmpty();
    }
}