using Microsoft.EntityFrameworkCore;
using MyLibrary.Models;

namespace MyLibrary.DAL
{
    public class LibraryDb: DbContext
    {
        public LibraryDb(DbContextOptions<LibraryDb> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<MyLibrary.Models.Book> Book { get; set; } = default!;
        public DbSet<MyLibrary.Models.Genre> Genre { get; set; } = default!;
        public DbSet<MyLibrary.Models.Shelf> Shelf { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Shelf)
                .WithMany(s => s.Books)
                .HasForeignKey(b => b.ShelfId)
                .OnDelete(DeleteBehavior.ClientCascade); 

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Shelf>()
                .HasOne(s => s.Genre)
                .WithMany(g => g.shelves)
                .HasForeignKey(s => s.GenreId)
                .OnDelete(DeleteBehavior.Restrict); 

            base.OnModelCreating(modelBuilder);
        }
    }
}
