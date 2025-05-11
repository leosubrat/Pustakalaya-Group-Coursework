using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pustakalaya.DTOs;
using Pustakalaya.Services.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pustakalaya.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto bookDto)
        {
            var book = await _bookService.CreateBookAsync(bookDto);
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDto>> UpdateBook(int id, UpdateBookDto bookDto)
        {
            var book = await _bookService.UpdateBookAsync(id, bookDto);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("{id}/stock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDto>> UpdateBookStock(int id, [FromBody] int quantity)
        {
            var book = await _bookService.UpdateBookStockAsync(id, quantity);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPut("{id}/discount")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDto>> ApplyDiscount(int id, DiscountDto discountDto)
        {
            var book = await _bookService.ApplyDiscountAsync(id, discountDto);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpDelete("{id}/discount")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDto>> RemoveDiscount(int id)
        {
            var book = await _bookService.RemoveDiscountAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDto>>> SearchBooks([FromQuery] string term)
        {
            var books = await _bookService.SearchBooksAsync(term);
            return Ok(books);
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<object>> GetPaginatedBooks(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = null,
            [FromQuery] string sortBy = "Title",
            [FromQuery] bool ascending = true)
        {
            var result = await _bookService.GetPaginatedBooksAsync(pageNumber, pageSize, searchTerm, sortBy, ascending);

            return Ok(new
            {
                Books = result.Books,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            });
        }
    }
}