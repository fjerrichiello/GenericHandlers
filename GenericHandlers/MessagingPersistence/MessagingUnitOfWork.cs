using Common.Messaging.Persistence;
using GenericHandlers.Persistence.UnitOfWork;

namespace GenericHandlers.MessagingPersistence;

public class MessagingUnitOfWork(ApplicationMessagingDbContext context)
    : UnitOfWork<ApplicationMessagingDbContext>(context), IMessagingUnitOfWork;
