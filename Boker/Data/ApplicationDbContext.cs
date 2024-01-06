using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Boker.Models;

namespace Boker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Boker.Models.Book>? Book { get; set; }
        public DbSet<Boker.Models.BookReview>? BookReview { get; set; }
        public DbSet<Boker.Models.BookType>? BookType { get; set; }
        public DbSet<Boker.Models.MyLib>? MyLib { get; set; }
    }
}