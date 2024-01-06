using Microsoft.AspNetCore.Identity;

namespace Boker.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int TotalPages { get; set; }
        public int BookTypeId { get; set; }
        public virtual BookType? BookType { get; set; }
        public string? userId { get; set; }
        public virtual IdentityUser? User { get; set; }
        public virtual ICollection<BookReview> Reviews { get; set; }
    }
}
