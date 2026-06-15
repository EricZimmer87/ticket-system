using System.ComponentModel.DataAnnotations;
using TicketSystem.Models;

namespace TicketSystem.DTOs.Tickets
{
    public class CreateTicketRequest
    {

        public Priority Priority { get; set; } = Priority.Medium;

        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; } = string.Empty;
    }
}
