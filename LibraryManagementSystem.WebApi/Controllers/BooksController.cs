using LibraryManagementSystem.Application.Dtos;
using LibraryManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDto>> GetBookById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto createBookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var newBook = await _bookService.CreateBookAsync(createBookDto);
                return CreatedAtAction(nameof(GetBookById), new { id = newBook.ID }, newBook);
            }
            catch (Exception ex) 
            {
                               return StatusCode(500, "An error occurred while creating the book.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] CreateBookDto updateBookDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _bookService.UpdateBookAsync(id, updateBookDto);
            return NoContent(); 
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteBookAsync(id); 
            return NoContent();
        }


        // --- Borrowing Endpoints ---
        [HttpPost("borrow")]
        public async Task<ActionResult<BorrowTransactionDto>> BorrowBook([FromBody] CreateBorrowTransactionDto borrowDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var transaction = await _bookService.BorrowBookAsync(borrowDto);
            if (transaction == null)
            {
                return BadRequest(new { message = "Book not available or not found." });
            }
            return Ok(transaction);
        }

        [HttpPost("return/{borrowTransactionId:int}")]
        public async Task<IActionResult> ReturnBook(int borrowTransactionId)
        {
            await _bookService.ReturnBookAsync(borrowTransactionId);
            return Ok(new { message = "Book return processed." }); 
        }
    }
}
