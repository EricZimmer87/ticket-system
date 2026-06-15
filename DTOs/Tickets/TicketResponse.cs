using TicketSystem.Models;

namespace TicketSystem.DTOs.Tickets
{
    public class TicketResponse
    {
        public int TicketId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public string? UpdatedByUserName { get; set; }

        public Priority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; }
    }
}
