using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketSystem.Data;
using TicketSystem.DTOs.Comments;
using TicketSystem.DTOs.Shared;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/Comments/5 get comment by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentResponse>> GetCommentById(int id)
        {
            var response = await _context.Comments
                .AsNoTracking()
                .Where(c => c.CommentId == id)
                .Select(c => new CommentResponse
                {
                    CommentId = c.CommentId,
                    TicketId = c.TicketId,
                    UserId = c.AppUserId,
                    Message = c.Message,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (response == null)
                return NotFound();

            return Ok(response);
        }

        // GET /api/tickets/{ticketId}/comments gets all comments for a specific ticket
        [HttpGet("/api/Tickets/{ticketId}/comments")]
        public async Task<ActionResult<PagedResponse<CommentResponse>>> GetCommentsByTicketId(int ticketId, int pageNumber = 1, int pageSize = 10)
        {
            // pageNumber and pageSize must be greater than 0
            if (pageNumber <= 0)
                return BadRequest($"{nameof(pageNumber)} must be greater than 0.");
            if (pageSize <= 0)
                return BadRequest($"{nameof(pageSize)} must be greater than 0.");

            // Max pageSize is 100
            if (pageSize > 100)
                return BadRequest("Maximum page size is 100.");

            // Ensure ticket exists
            var ticketExists = await _context.Tickets
                .AnyAsync(t => t.TicketId == ticketId);
            if (!ticketExists)
                return NotFound();

            var commentsQuery = _context.Comments
                .AsNoTracking()
                .Where(c => c.TicketId == ticketId);

            var totalComments = await commentsQuery.CountAsync();

            var comments = await commentsQuery
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CommentResponse
                {
                    CommentId = c.CommentId,
                    TicketId = c.TicketId,
                    UserId = c.AppUserId,
                    Message = c.Message,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalComments / (double)pageSize);

            var response = new PagedResponse<CommentResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalComments,
                TotalPages = totalPages,
                HasNextPage = pageNumber < totalPages,
                HasPreviousPage = pageNumber > 1,
                Items = comments
            };

            return Ok(response);
        }

        // POST /api/tickets/{ticketId}/comments creates a new comment for a specific ticket
        [HttpPost("/api/Tickets/{ticketId}/comments")]
        public async Task<ActionResult<CommentResponse>> CreateComment(int ticketId, CreateCommentRequest request)
        {
            var ticketExists = await _context.Tickets
                .AnyAsync(t => t.TicketId == ticketId);
            if (!ticketExists)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            DateTime now = DateTime.UtcNow;

            var comment = new Comment
            {
                TicketId = ticketId,
                AppUserId = userId,
                Message = request.Message,
                CreatedAt = now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var response = new CommentResponse
            {
                CommentId = comment.CommentId,
                TicketId = ticketId,
                UserId = userId,
                Message = comment.Message,
                CreatedAt = now
            };

            return CreatedAtAction(
                nameof(GetCommentById),
                new { id = comment.CommentId },
                response);
        }

        // DELETE /api/comments/{id} deletes a comment by ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.CommentId == id);
            if (comment == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();
            if (comment.AppUserId != userId)
                return Forbid();

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
