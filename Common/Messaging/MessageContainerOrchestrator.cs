using Dumpify;

namespace Common.Messaging;

public class MessageContainerOrchestrator<TMessage, TMessageMetadata>(
    MessageContainerMapper<TMessage, TMessageMetadata> _mapper,
    IMessageContainerHandler<TMessage, TMessageMetadata> _handler)
    : IMessageOrchestrator
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    public async Task ProcessAsync(MessageRequest request)
    {
        var container = _mapper.ToMessageContainer(request);

        container.Dump();

        await _handler.HandleAsync(container);
    }
}