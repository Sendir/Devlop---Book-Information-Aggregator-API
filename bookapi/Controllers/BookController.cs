using bookapi.Dtos.Books;
using bookapi.Dtos.OpenLibrary;
using bookapi.Interfaces.IServices;
using bookapi.Mappers;
using bookapi.Query;
using Microsoft.AspNetCore.Mvc;

namespace bookapi.Controllers
{
    [ApiController]
    [Route("books")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public record ErrorResponse(string Message);

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<BookDto>> GetAllLocalBooks()
        {
            var books = _bookService.GetAllBooks().Select(book => book.ToBookDto()).ToList();

            if (books.Count == 0)
            {
                return NotFound(new ErrorResponse("Couldn't find any books in the collection."));
            }

            return Ok(books);
        }

        [HttpPost]
        public ActionResult<BookDto> AddLocalBook([FromBody] CreateBookDto createBookDto)
        {
            var error = ValidateCreateBookDto(createBookDto);
            if (error != null) return BadRequest(error);

            try
            {
                var bookDto = _bookService.CreateLocalBook(createBookDto);
                return Created(bookDto.Id.ToString(), bookDto);
            }
            catch (InvalidOperationException ioe)
            {
                return Conflict(new ErrorResponse(ioe.Message));
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetOpenLibrarySearch([FromQuery] string query, [FromQuery] PaginationOptions pagination)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest(new ErrorResponse("Query parameter cannot be empty or null."));
            }

            var error = ValidatePaginationOptions(pagination);
            if (error != null) return BadRequest(error);

            var openLibraryBooks = await _bookService.SearchBooksAsync(query, pagination);

            return Ok(openLibraryBooks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OpenLibraryDetailsDto>> GetBookDetails(string id)
        {
            var bookDetail = await _bookService.SearchBookById(id);

            var response = bookDetail.toOpenLibraryDetailsDtoFromJsonDocument();
            response.KeyWork = id;

            return Ok(response);
        }

        private ErrorResponse? ValidateCreateBookDto(CreateBookDto createBookDto)
        {
            if (createBookDto == null) return new ErrorResponse("Request body cannot be null.");
            if (string.IsNullOrWhiteSpace(createBookDto.Title)) return new ErrorResponse("Title is required.");
            if (string.IsNullOrWhiteSpace(createBookDto.Author)) return new ErrorResponse("Author is required.");
            if (createBookDto.PublishedYear <= 0) return new ErrorResponse("Published year must be a positive number.");
            if (createBookDto.Description != null && createBookDto.Description.Length > 500)
                return new ErrorResponse("Description cannot exceed 500 characters.");
            return null;
        }

        private ErrorResponse? ValidatePaginationOptions(PaginationOptions paginationOptions)
        {
            if (paginationOptions.PageNumber <= 0) return new ErrorResponse("Page number must be a positive number.");
            if (paginationOptions.PageSize <= 0) return new ErrorResponse("Page size must be a positive number.");
            return null;
        }
    }

}