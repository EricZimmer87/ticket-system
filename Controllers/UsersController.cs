using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Data;
using TicketSystem.DTOs.Users;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetUsers()
        {
            var users = await _context.Users
                .AsNoTracking()
                .ToListAsync();

            var response = new List<UserResponse>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                response.Add(new UserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles.ToList(),
                    TicketsCreated = user.CreatedTickets.Count
                });
            }

            return Ok(response);
        }

        // POST /api/Users/{id}/roles
        [HttpPost("{id}/roles")]
        public async Task<ActionResult> AddRoleToUser(string id, AddRoleToUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (!await _roleManager.RoleExistsAsync(request.Role))
                return NotFound("Role does not exist.");

            if (await _userManager.IsInRoleAsync(user, request.Role))
                return BadRequest("User already has this role.");

            // Cannot remove the last admin
            var admins = await _userManager.GetUsersInRoleAsync(Roles.Admin);
            if (admins.Count == 1 && admins[0].Id == user.Id)
                return BadRequest("Cannot remove the last administrator.");

            var result = await _userManager.AddToRoleAsync(user, request.Role);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
    }
}
