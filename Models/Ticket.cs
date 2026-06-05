namespace TicketSystem.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public Priority Priority { get; set; } = Priority.Medium;

        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = [];
    }
}
