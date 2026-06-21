using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketSystem.Data;
using TicketSystem.DTOs.Shared;
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
            /*
             Load related users because the response uses navigation properties.
             Without .Include(), EF Core only loads the Ticket entity. Projecting
             directly to a DTO is an alternative that generates the required JOINs.
             I chose to load the Ticket entity first so I can validate ownership and
             return Forbid() when appropriate instead of projecting directly to a DTO.
            */

            var ticket = await _context.Tickets
                .AsNoTracking()
                .Include(t => t.CreatedByUser)
                .Include(t => t.UpdatedByUser)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!User.IsInRole(Roles.Admin) &&
                ticket.CreatedByUserId != userId)
            {
                return Forbid();
            }

            var response = new TicketResponse
            {
                TicketId = ticket.TicketId,
                CreatedByUserName = ticket.CreatedByUser.UserName!,
                UpdatedByUserName = ticket.UpdatedByUser?.UserName,
                UpdatedAt = ticket.UpdatedAt,
                Priority = ticket.Priority,
                CreatedAt = ticket.CreatedAt,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status
            };

            return Ok(response);
        }

        // GET /api/Tickets gets all tickets
        [HttpGet]
        public async Task<ActionResult<PagedResponse<TicketResponse>>> GetTickets(int pageNumber = 1, int pageSize = 10)
        {
            // pageNumber and pageSize must be greater than 0
            if (pageNumber <= 0)
                return BadRequest($"{nameof(pageNumber)} must be greater than 0.");
            if (pageSize <= 0)
                return BadRequest($"{nameof(pageSize)} must be greater than 0.");

            // Max pageSize is 100
            if (pageSize > 100)
                return BadRequest("Maximum page size is 100.");

            // Start query
            IQueryable<Ticket> query = _context.Tickets.AsNoTracking();

            // Get user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If the current user is not an admin, only show tickets created by the currently logged in user
            if (!User.IsInRole(Roles.Admin))
            {
                query = query.Where(t => t.CreatedByUserId == userId);

            }

            // Get total tickets count
            var totalTickets = await query.CountAsync();

            // Finish query, project into responses
            var ticketResponses = await query
                .OrderBy(t => t.TicketId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TicketResponse
                {
                    TicketId = t.TicketId,
                    CreatedByUserName = t.CreatedByUser.UserName ?? "",
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
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalTickets / (double)pageSize);

            var response = new PagedResponse<TicketResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalTickets,
                TotalPages = totalPages,
                HasNextPage = pageNumber < totalPages,
                HasPreviousPage = pageNumber > 1,
                Items = ticketResponses
            };

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
            // Get ticket
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
                return NotFound();

            //Owner or Admin ?
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole(Roles.Admin) && userId != ticket.CreatedByUserId)
                return Forbid();

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
        [Authorize(Roles = Roles.Admin)]
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
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
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
