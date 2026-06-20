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
    [Authorize(Roles = Roles.Admin)]
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

        // GET /api/Users gets all users with their roles
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
                    Role = roles.SingleOrDefault(),
                });
            }

            return Ok(response);
        }

        // PUT /api/Users/{id}/roles change a user's role
        [HttpPut("{id}/role")]
        public async Task<ActionResult> ChangeUserRole(string id, ChangeUserRoleRequest request)
        {
            // Get user
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Ensure the role to change to exists
            if (!await _roleManager.RoleExistsAsync(request.Role))
                return NotFound("Role does not exist.");

            // Get the user's role
            var currentRoles = await _userManager.GetRolesAsync(user);
            var currentRole = currentRoles.SingleOrDefault(); // user should have only one role
            // If the user already has the requested role, no need to change it
            if (currentRole == request.Role)
                return BadRequest("User already has this role.");

            // Cannot remove the last admin
            if (currentRole == Roles.Admin &&
                request.Role != Roles.Admin)
            {
                var admins = await _userManager.GetUsersInRoleAsync(Roles.Admin);

                // If there is only one admin and that one admin is the user in the request...
                if (admins.Count == 1 &&
                    admins[0].Id == user.Id)
                {
                    return BadRequest("Cannot remove the last administrator.");
                }
            }

            // Remove current role
            if (currentRole != null)
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, currentRole);

                if (!removeResult.Succeeded)
                    return BadRequest(removeResult.Errors);
            }

            // Add requested role
            var result = await _userManager.AddToRoleAsync(user, request.Role);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
    }
}
