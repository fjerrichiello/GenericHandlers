using Common.Messaging.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GenericHandlers.MessagingPersistence;

public class ApplicationMessagingDbContext(DbContextOptions<ApplicationMessagingDbContext> options)
    : MessagingContext<ApplicationMessagingDbContext>(options)
{
}