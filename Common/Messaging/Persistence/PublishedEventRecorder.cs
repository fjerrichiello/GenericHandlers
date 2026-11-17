using Common.Messaging.Persistence.Models;
using Common.Transactions;

namespace Common.Messaging.Persistence;

/// <summary>
/// Stores published events and completes the messaging unit of work. Resolved inside an
/// <see cref="ITransaction{T}"/> scope, so every call gets a fresh repository and unit of work.
/// </summary>
public class PublishedEventRecorder(
    IMessagingRepository _messagingRepository,
    IMessagingUnitOfWork _unitOfWork)
{
    public async Task<Result> RecordAsync(IEnumerable<PublishedEvent> publishedEvents)
    {
        foreach (var publishedEvent in publishedEvents)
        {
            _messagingRepository.AddPublishedEvent(publishedEvent);
        }

        await _unitOfWork.CompleteAsync();

        return new Result(true);
    }
}
