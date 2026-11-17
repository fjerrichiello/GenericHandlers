using Common.Messaging.Persistence.Models;

namespace Common.Messaging.Persistence;

public interface IMessagingRepository
{
    void AddPublishedEvent(PublishedEvent publishedEvent);
    
    void AddReceivedEvent(ReceivedEvent receivedEvent);
    
    void AddReceivedCommand(ReceivedCommand receivedCommand);
}