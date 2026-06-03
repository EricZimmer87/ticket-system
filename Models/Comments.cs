namespace TicketSystem.Models
{
    public class Comments
    {
        public int CommentId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public int TicketId { get; set; }
        public Tickets Ticket { get; set; } = null!;

        public int UserId { get; set; }
        public Users User { get; set; } = null!;
    }
}
