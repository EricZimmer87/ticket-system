using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Data;
using TicketSystem.DTOs.User;

namespace TicketSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetUsers()
        {
            var response = await _context.Users
                .Select(u => new UserResponse
                {
                    UserId = u.UserId,
                    Username = u.Username
                })
                .ToListAsync();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUserById(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();

            var response = new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username
            };

            return Ok(response);
        }
    }
}
