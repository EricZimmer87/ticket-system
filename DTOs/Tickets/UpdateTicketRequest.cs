using TicketSystem.Models;

namespace TicketSystem.DTOs.Tickets
{
    public class UpdateTicketRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; }
        public Priority Priority { get; set; }

    }
}
