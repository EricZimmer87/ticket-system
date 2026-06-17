using System.ComponentModel.DataAnnotations;
using TicketSystem.Models;

namespace TicketSystem.DTOs.Tickets
{
    public class UpdateTicketRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public TicketStatus Status { get; set; }

        [Required]
        public Priority Priority { get; set; }

    }
}
