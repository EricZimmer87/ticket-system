namespace TicketSystem.DTOs.Comments
{
    public class CommentResponse
    {
        public int CommentId { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
