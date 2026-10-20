using LibraryApplicationApi.Data;
using LibraryApplicationApi.Dtos.LibraryApi.DTOs;
using LibraryApplicationApi.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LibraryApplicationApi.Controllers
{
    public class BorrowingController : Controller
    {
        [Authorize]
        [ApiController]
            [Route("api/[controller]")]
            public class BorrowingsController : ControllerBase
            {
            private readonly LibraryDbContext _context;
            private readonly ILogger<BorrowingsController> _logger;

            public BorrowingsController(
                LibraryDbContext context,
                ILogger<BorrowingsController> logger)
            {
                _context = context;
                _logger = logger;
            }

            [HttpGet]
            public async Task<ActionResult<IEnumerable<Borrowing>>> GetBorrowings()
            {
                var borrowings = await _context.Borrowings
                    .AsNoTracking()
                    .Include(b => b.Book)
                    .Include(b => b.Member)
                    .ToListAsync();

                return Ok(borrowings);
            }

            [HttpGet("{id:int}")]
            public async Task<ActionResult<Borrowing>> GetBorrowing(int id)
            {
                var borrowing = await _context.Borrowings
                    .AsNoTracking()
                    .Include(b => b.Book)
                    .Include(b => b.Member)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (borrowing == null)
                {
                    return NotFound(new { message = "Borrowing record not found." });
                }

                return Ok(borrowing);
            }
            [Authorize(Roles = "Admin,Member")]
            [HttpPost]
            public async Task<ActionResult<Borrowing>> CreateBorrowing(
                CreateBorrowingRequest request)
            {
                if (request.DueDate.Date < DateTime.UtcNow.Date)
                {
                    return BadRequest(new
                    {
                        message = "The due date cannot be in the past."
                    });
                }

                var book = await _context.Books.FindAsync(request.BookId);

                if (book == null)
                {
                    return NotFound(new { message = "Book not found." });
                }

                var member = await _context.Members.FindAsync(request.MemberId);

                if (member == null)
                {
                    return NotFound(new { message = "Member not found." });
                }

                if (!book.IsAvailable)
                {
                    return Conflict(new
                    {
                        message = "This book is currently unavailable."
                    });
                }

                var borrowing = new Borrowing
                {
                    BookId = book.Id,
                    MemberId = member.MemberId,
                    BorrowedDate = DateTime.UtcNow,
                    ReturnedDate = request.DueDate,

                };

                book.IsAvailable = false;

                _context.Borrowings.Add(borrowing);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Borrowing record {BorrowingId} created for book {BookId}",
                    borrowing.Id,
                    book.Id);

                return CreatedAtAction(
                    nameof(GetBorrowing),
                    new { id = borrowing.Id },
                    borrowing);
            }
            [Authorize(Roles = "Admin,Member")]
            [HttpPut("{id:int}/return")]
                public async Task<IActionResult> ReturnedBook(int id)
                {
                    var borrowing = await _context.Borrowings
                        .Include(b => b.Book)
                        .FirstOrDefaultAsync(b => b.Id == id);

                    if (borrowing == null)
                    {
                        return NotFound(new
                        {
                            message = "Borrowing record not found."
                        });
                    }

                    if (borrowing.ReturnedDate != null)
                    {
                        return BadRequest(new
                        {
                            message = "This book has already been returned."
                        });
                    }

                    if (borrowing.Book == null)
                    {
                        return NotFound(new
                        {
                            message = "The associated book was not found."
                        });
                    }

                    borrowing.ReturnedDate = DateTime.UtcNow;
                    borrowing.Book.IsAvailable = true;

                    await _context.SaveChangesAsync();

                    _logger.LogInformation(
                        "Book returned for borrowing record {BorrowingId}, book {BookId}",
                        borrowing.Id,
                        borrowing.BookId);

                    return Ok(new
                    {
                        message = "Book returned successfully.",
                        borrowingId = borrowing.Id,
                        bookId = borrowing.BookId,
                        returnDate = borrowing.ReturnedDate
                    });
                }

            }
        }
    }

    
