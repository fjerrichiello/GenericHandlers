using Microsoft.EntityFrameworkCore;

namespace GenericHandlers.Persistence;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<AuthorEntity> Authors { get; set; } = null!;

    public virtual DbSet<BookEntity> Books { get; set; } = null!;

    public virtual DbSet<BookRequestEntity> BookRequests { get; set; } = null!;

    public ApplicationDbContext(
        DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookRequestEntity>()
            .Property(x => x.RequestType)
            .HasConversion<string>();

        modelBuilder.Entity<BookRequestEntity>()
            .Property(x => x.ApprovalStatus)
            .HasConversion<string>();


        modelBuilder.Entity<BookRequestEntity>()
            .HasIndex(b => new { b.AuthorId, b.Title, b.ApprovalStatus })
            .IsUnique()
            .HasFilter("""
                       "ApprovalStatus" = 'Pending'
                       """);

        modelBuilder.Entity<BookRequestEntity>()
            .HasIndex(b => new { b.AuthorId, b.NewTitle, b.ApprovalStatus })
            .IsUnique()
            .HasFilter("""
                       "ApprovalStatus" = 'Pending'
                       """);

        modelBuilder.Entity<BookEntity>()
            .HasIndex(b => new { b.AuthorId, b.Title })
            .IsUnique();

        // List<AuthorEntity> authors =
        // [
        //     new Author() { Id = 1, AuthorId = "Dr.Seuss" },
        //     new Author() { Id = 2, AuthorId = "Roald Dahl" },
        //     new Author() { Id = 3, AuthorId = "Beatrix Potter" },
        //     new Author() { Id = 4, AuthorId = "Maurice Sendak" },
        //     new Author() { Id = 5, AuthorId = "Eric Carle" },
        //     new Author() { Id = 6, AuthorId = "Shel Silverstein" },
        //     new Author() { Id = 7, AuthorId = "Judy Blume" }
        // ];


        // modelBuilder.Entity<AuthorEntity>()
        //     .HasData(authors);
    }
}