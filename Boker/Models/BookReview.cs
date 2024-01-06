using Microsoft.AspNetCore.Identity;
using System;

namespace Boker.Models
{
    public class BookReview
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public virtual Book? Book { get; set; }
        public string? UserId { get; set; }
        public virtual IdentityUser? User { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
