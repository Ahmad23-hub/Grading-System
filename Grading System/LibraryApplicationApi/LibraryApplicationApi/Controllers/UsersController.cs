using LibraryApplicationApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            LibraryDbContext context,
            ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.ApplicationUsers
                .AsNoTracking()
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("{id:int}/role")]
        public async Task<IActionResult> UpdateRole(
            int id,
            [FromBody] string role)
        {
            var validRoles = new[] { "Admin", "Librarian", "Member" };

            if (!validRoles.Contains(role))
            {
                return BadRequest(new
                {
                    message = "Invalid role. Use Admin, Librarian, or Member."
                });
            }

            var user = await _context.ApplicationUsers.FindAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            user.Role = role;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "User {UserId} role changed to {Role}",
                user.Id,
                user.Role);

            return Ok(new
            {
                message = "User role updated successfully.",
                userId = user.Id,
                role = user.Role
            });
        }
    }
}