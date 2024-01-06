using Microsoft.AspNetCore.Identity;
using System;

namespace Boker.Models
{
    public class MyLib
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsFavorite { get; set; }
        public string UserId { get; set; }
        public virtual IdentityUser? user { get; set; }

        public DateTime? StartedReading { get; set; }
        public DateTime? FinishedReading { get; set; }
        public string Notes { get; set; }
    }
}
