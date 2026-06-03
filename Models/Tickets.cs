namespace TicketSystem.Models
{
    public class Tickets
    {
        public int TicketId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Open";
    }
}
