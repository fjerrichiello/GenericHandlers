using Common.Messaging.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Messaging.Persistence;

public abstract class MessagingContext<TContext>(DbContextOptions<TContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    where TContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<PublishedEvent> PublishedEvents { get; set; }

    public DbSet<ReceivedCommand> ReceivedCommands { get; set; }

    public DbSet<ReceivedEvent> ReceivedEvents { get; set; }
}