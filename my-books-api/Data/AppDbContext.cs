using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using my_books_api.Data.Models;

namespace my_books_api.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book_Author>()
                .HasOne(a => a.Book)
                .WithMany(bi=> bi.Book_Author)
                .HasForeignKey(a=>a.BookId);
            modelBuilder.Entity<Book_Author>()
               .HasOne(a => a.Author)
               .WithMany(bi => bi.Book_Author)
               .HasForeignKey(a => a.AuthorId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet <Book_Author> AuthorBooks { get; set;}
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

    }
}
