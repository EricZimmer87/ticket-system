using Microsoft.AspNetCore.Identity;

namespace TicketSystem.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Ticket> Tickets { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
    }
}
