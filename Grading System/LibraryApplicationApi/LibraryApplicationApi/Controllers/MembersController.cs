using LibraryApplicationApi.Data;
using LibraryApplicationApi.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApplicationApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly ILogger<MembersController> _logger;

        public MembersController(
            LibraryDbContext context,
            ILogger<MembersController> logger)
        {
            _context = context;
            _logger = logger;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            _logger.LogInformation("Retrieving all members.");

            return await _context.Members.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                _logger.LogWarning(
                    "Member with ID {MemberId} was not found.",
                    id);

                return NotFound(new
                {
                    message = "Member not found."
                });
            }

            _logger.LogInformation(
                "Retrieved member with ID {MemberId}.",
                id);

            return member;
        }

        [Authorize(Roles = "Admin,Member")]
        [HttpPost]
        public async Task<ActionResult<Member>> CreateMember(Member member)
        {
            _context.Members.Add(member);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Member with ID {MemberId} was registered.",
             member.MemberId);

            return CreatedAtAction(
                nameof(GetMember),
                new { id = member.MemberId },
                member
            );
        }

        [Authorize(Roles = "Admin,Member")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(
            int id, Member member)
        {
            if (id != member.MemberId)
            {
                _logger.LogWarning(
                    "Member update rejected because URL ID {UrlId} " +
                    "does not match body ID {BodyId}.",
                    id,
                    member.MemberId);

                return BadRequest(new
                {
                    message = "The URL ID and member ID must match."
                });
            }

            var existingMember =
                await _context.Members.FindAsync(id);

            if (existingMember == null)
            {
                _logger.LogWarning(
                    "Cannot update member {MemberId}: member not found.",
                    id);

                return NotFound(new
                {
                    message = "Member not found."
                });
            }

            existingMember.FullName = member.FullName;
            existingMember.Email = member.Email;
            existingMember.PhoneNumber = member.PhoneNumber;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Member with ID {MemberId} was updated.",
                id);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                _logger.LogWarning(
                    "Cannot delete member {MemberId}: member not found.",
                    id);

                return NotFound(new
                {
                    message = "Member not found."
                });
            }

            bool hasBorrowings = await _context.Borrowings
                .AnyAsync(b => b.MemberId == id);

            if (hasBorrowings)
            {
                _logger.LogWarning(
                    "Cannot delete member {MemberId}: borrowing records exist.",
                    id);

                return Conflict(new
                {
                    message =
                        "Cannot delete a member with borrowing records."
                });
            }

            _context.Members.Remove(member);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Member with ID {MemberId} was deleted.",
                id);

            return NoContent();
        }
    }
}