using Microsoft.AspNetCore.Identity;

namespace Boker.Models
{
    public class BookType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string userId { get; set; }
        public virtual IdentityUser? user { get; set; }
    }
}
