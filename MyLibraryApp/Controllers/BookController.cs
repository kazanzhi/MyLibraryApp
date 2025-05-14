using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLibraryApp.Dtos;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;

namespace MyLibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public BookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet("{bookId}")]
        [AllowAnonymous]
        public async Task<ActionResult<Book>> GetBook(int bookId)
        {
            var book = await _bookRepository.GetBook(bookId);
            if (book is null)
                return NotFound($"Book with Id: {bookId} not found.");

            return Ok(book);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Book>>> GetBooks()
        {
            var books = await _bookRepository.GetBooks();

            if (!books.Any())
                return NoContent();

            return Ok(books);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<Book>> CreateBook([FromBody] BookDto bookDto)
        {
            var createdBook = await _bookRepository.CreateBook(bookDto);

            if (createdBook is null)
                return Conflict("This book already exist or current category does not exist.");

            return CreatedAtAction(nameof(GetBook), new { bookId = createdBook.Id }, createdBook);
        }

        [HttpPut("{bookId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> UpdateBook(int bookId, [FromBody] BookDto bookDto)
        {
            var updatedBook = await _bookRepository.UpdateBook(bookId, bookDto);

            if (updatedBook is 0)
                return NotFound($"Book with Id {bookId} not found or current category does not exists.");

            return NoContent();
        }

        [HttpDelete("{bookId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> DeleteBook(int bookId)
        {
            var deletedBook = await _bookRepository.DeleteBook(bookId);

            if(deletedBook is 0)
                return NotFound($"Book with Id {bookId} not found.");

            return NoContent();
        }
    }
}
