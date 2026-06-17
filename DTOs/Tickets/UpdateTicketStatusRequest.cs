using System.ComponentModel.DataAnnotations;
using TicketSystem.Models;

namespace TicketSystem.DTOs.Tickets
{
    public class UpdateTicketStatusRequest
    {
        [Required]
        public TicketStatus Status { get; set; }
    }
}
