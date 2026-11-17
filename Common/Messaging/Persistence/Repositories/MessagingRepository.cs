using Common.Messaging.Persistence.Models;

namespace Common.Messaging.Persistence.Repositories;

public class MessagingRepository<TContext>(TContext _context)
    : IMessagingRepository
    where TContext : MessagingContext<TContext>
{
    public void AddPublishedEvent(PublishedEvent publishedEvent)
    {
        _context.PublishedEvents.Add(publishedEvent);
    }

    public void AddReceivedCommand(ReceivedCommand receivedCommand)
    {
        _context.ReceivedCommands.Add(receivedCommand);
    }

    public void AddReceivedEvent(ReceivedEvent receivedEvent)
    {
        _context.ReceivedEvents.Add(receivedEvent);
    }
}
