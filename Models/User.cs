namespace TicketSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<Comment> Comments { get; set; } = [];

        public ICollection<Ticket> Tickets { get; set; } = [];

        public ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<Role> Roles { get; set; } = [];
    }
}
