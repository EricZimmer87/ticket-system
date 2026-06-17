using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketSystem.Data;
using TicketSystem.DTOs.Tickets;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/Tickets/5 gets ticket by ID
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
                    UpdatedAt = t.UpdatedAt,
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

        // GET /api/Tickets gets all tickets
        [HttpGet]
        public async Task<ActionResult<List<TicketResponse>>> GetTickets()
        {
            var response = await _context.Tickets
                .AsNoTracking()
                .Select(t => new TicketResponse
                {
                    TicketId = t.TicketId,
                    CreatedByUserName = t.CreatedByUser.UserName ?? "",
                    UpdatedByUserName = t.UpdatedByUser != null
                        ? t.UpdatedByUser.UserName : null,
                    UpdatedAt = t.UpdatedAt,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status
                })
                .ToListAsync();

            return Ok(response);
        }

        // POST /api/Tickets creates a new ticket
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
                UpdatedByUserId = null,
                UpdatedAt = null,
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
                Status = newTicket.Status,
                UpdatedAt = newTicket.UpdatedAt
            };

            return CreatedAtAction(
                nameof(GetTicketById),
                new { id = response.TicketId },
                response);
        }

        // PUT /api/Tickets/5 updates a ticket
        [HttpPut("{id}")]
        public async Task<ActionResult<TicketResponse>> UpdateTicket(UpdateTicketRequest request, int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var ticket = await _context.Tickets
                .Where(t => t.TicketId == id)
                .FirstOrDefaultAsync();

            if (ticket == null)
                return NotFound();

            ticket.Title = request.Title;
            ticket.Description = request.Description;
            ticket.Status = request.Status;
            ticket.Priority = request.Priority;
            ticket.UpdatedByUserId = userId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Requery the database to project the updated ticket and related user names into the response DTO
            var response = await _context.Tickets
                .AsNoTracking()
                .Where(t => t.TicketId == id)
                .Select(t => new TicketResponse
                {
                    TicketId = t.TicketId,
                    CreatedByUserName = t.CreatedByUser.UserName ?? "",
                    UpdatedByUserName = t.UpdatedByUser != null
                        ? t.UpdatedByUser.UserName : null,
                    UpdatedAt = t.UpdatedAt,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status
                })
                .FirstAsync();

            return Ok(response);
        }

        // PATCH /api/tickets/5/status updates a ticket's status
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<TicketResponse>> UpdateTicketStatus(UpdateTicketStatusRequest request, int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var ticket = await _context.Tickets
                .Where(t => t.TicketId == id)
                .FirstOrDefaultAsync();
            if (ticket == null)
                return NotFound();

            ticket.Status = request.Status;
            ticket.UpdatedByUserId = userId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = await _context.Tickets
                .AsNoTracking()
                .Where(t => t.TicketId == id)
                .Select(t => new TicketResponse
                {
                    TicketId = t.TicketId,
                    CreatedByUserName = t.CreatedByUser.UserName ?? "",
                    UpdatedByUserName = t.UpdatedByUser != null
                        ? t.UpdatedByUser.UserName : null,
                    UpdatedAt = t.UpdatedAt,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status
                })
                .FirstAsync();

            return Ok(response);
        }

        // DELETE /api/tickets/5 deletes a ticket
        // TODO: [Authorize("{Role: Admin}")]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
                return NotFound();

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
