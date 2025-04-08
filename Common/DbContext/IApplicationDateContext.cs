using Microsoft.EntityFrameworkCore;

namespace Common.DbContext;

public interface IApplicationDateContext
{
    DbSet<ApplicationDate> ApplicationDates { get; }
}