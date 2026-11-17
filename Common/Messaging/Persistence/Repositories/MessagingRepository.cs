using Common.Messaging.Persistence.Models;

namespace Common.Messaging.Persistence.Repositories;

public class MessagingRepository<TContext>(MessagingContext<TContext> _context)
    : IMessagingRepository
    where TContext : Microsoft.EntityFrameworkCore.DbContext
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