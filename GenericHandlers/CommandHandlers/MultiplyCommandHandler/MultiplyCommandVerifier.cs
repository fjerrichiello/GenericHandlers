using Common.Events.MultiplyCommand;
using Common.Messaging;
using Common.Verifiers;
using FluentValidation;
using FluentValidation.Results;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.MultiplyCommandHandler;

public class MultiplyCommandVerifier : AuthorizedCommandVerifier<MultiplyCommand, MultiplyCommandUnverifiedData>
{
    protected override void AuthorizationRules()
    {
        RuleFor(x => x.MessageContainer.Message)
            .NotEmpty();
    }

    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .GreaterThan(0);
    }
}