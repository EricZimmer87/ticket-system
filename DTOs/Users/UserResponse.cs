namespace TicketSystem.DTOs.Users
{
    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public List<string>? Roles { get; set; }
        public int TicketsCreated { get; set; }
    }
}
