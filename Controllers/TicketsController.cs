using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Data;
using TicketSystem.DTOs.Tickets;
using TicketSystem.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TicketSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponse>> GetTicketById(int id)
        {
            var response = await _context.Tickets
                .AsNoTracking()
                .Where(t => t.TicketId == id)
                .Select(t => new TicketResponse
                {
                    TicketId = t.TicketId,
                    CreatedByUserName = t.CreatedByUser.UserName!,
                    UpdatedByUserName = t.UpdatedByUser != null
                        ? t.UpdatedByUser.UserName
                        : null,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status
                })
                .FirstOrDefaultAsync();

            if (response == null)
                return NotFound();

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<List<TicketResponse>>> GetTickets()
        {
            var response = await _context.Tickets
                .Select(t => new TicketResponse
                {
                    TicketId = t.TicketId,
                    CreatedByUserName = t.CreatedByUser.UserName!,
                    UpdatedByUserName = t.UpdatedByUser != null 
                        ? t.UpdatedByUser.UserName : null,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status
                })
                .ToListAsync();

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TicketResponse>> CreateTicket(CreateTicketRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var newTicket = new Ticket
            {
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedByUserId = userId,
                Priority = request.Priority,
                Title = request.Title,
                Description = request.Description,
                Status = TicketStatus.Open
            };

            _context.Tickets.Add(newTicket);
            await _context.SaveChangesAsync();

            var response = new TicketResponse
            {
                TicketId = newTicket.TicketId,
                CreatedByUserName = User.Identity?.Name ?? "",
                Priority = newTicket.Priority,
                CreatedAt = newTicket.CreatedAt,
                Title = newTicket.Title,
                Description = newTicket.Description,
                Status = newTicket.Status
            };

            return CreatedAtAction(
                nameof(GetTicketById),
                new { id = response.TicketId },
                response);
        }
    }
}
