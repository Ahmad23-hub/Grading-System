using LibraryApplicationApi.Data;
using LibraryApplicationApi.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApplicationApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }
        [Authorize(Roles = "Admin,Member")]
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            _context.Books.Add(book);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetBook),
                new { id = book.Id },
                book
            );
        }
        [Authorize(Roles = "Admin,Member")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);

            await _context.SaveChangesAsync();

            return NoContent();
        }
            [HttpGet("search")] 
             
            public async Task<ActionResult<IEnumerable<Book>>> SearchBooks(
                [FromQuery] string term)
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest(new
                    {
                        message = "Please provide a search term."
                    });
                }

                term = term.Trim();

                var books = await _context.Books
                    .AsNoTracking()
                    .Where(b =>
                        b.Title.Contains(term) ||
                        b.Author.Contains(term) ||
                        b.ISBN.Contains(term))
                    .ToListAsync();

                return Ok(books);
            }

        }
    }

    
