using Microsoft.AspNetCore.Identity;

namespace TicketSystem.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Ticket> CreatedTickets { get; set; } = [];
        public ICollection<Ticket> UpdatedTickets { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
    }
}
