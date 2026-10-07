using LibraryApplicationApi.Data;
using LibraryApplicationApi.models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApplicationApi.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<Member> Members { get; set; }

        public DbSet<Borrowing> Borrowings { get; set; }
    }
}