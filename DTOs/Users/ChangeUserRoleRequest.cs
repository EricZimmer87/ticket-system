using System.ComponentModel.DataAnnotations;

namespace TicketSystem.DTOs.Users
{
    public class ChangeUserRoleRequest
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
