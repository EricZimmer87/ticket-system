using System.ComponentModel.DataAnnotations;

namespace TicketSystem.DTOs.Users
{
    public class AddRoleToUserRequest
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
