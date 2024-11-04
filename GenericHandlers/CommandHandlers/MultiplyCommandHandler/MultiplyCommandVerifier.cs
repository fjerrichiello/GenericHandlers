using Common.Messaging;
using Common.Verifiers;
using FluentValidation;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.MultiplyCommandHandler;

public class MultiplyCommandVerifier : MessageVerifier<MultiplyCommand, CommandMetadata, MultiplyCommandUnverifiedData>
{
    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .GreaterThan(0);
    }
}