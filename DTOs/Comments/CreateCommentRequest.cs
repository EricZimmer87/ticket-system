using System.ComponentModel.DataAnnotations;

namespace TicketSystem.DTOs.Comments
{
    public class CreateCommentRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Message { get; set; } = string.Empty;
    }
}
