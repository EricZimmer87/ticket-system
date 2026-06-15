namespace TicketSystem.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public Priority Priority { get; set; } = Priority.Medium;

        public string CreatedByUserId { get; set; } = string.Empty;
        public AppUser CreatedByUser { get; set; } = null!;

        public string? UpdatedByUserId { get; set; }
        public AppUser? UpdatedByUser { get; set; }

        public ICollection<Comment> Comments { get; set; } = [];
    }
}
