using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GenericHandlers.Persistence.UnitOfWork;

public class UnitOfWork<TDbContext>(TDbContext _context) : IUnitOfWork where TDbContext : DbContext
{
    private bool _completed = false;

    public async Task CompleteAsync()
    {
        if (_completed)
        {
            throw new InvalidOperationException("Already Completed");
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when ((exception.InnerException as PostgresException)?.SqlState == "23505")
        {
            throw new InvalidOperationException("Failed to Save", exception.InnerException);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrentUpdateException("Failed to update due to record already being changed");
        }

        _completed = true;
    }
}