namespace TicketSystem.Models
{
    public class UserRole
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }

        public Role Role { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
