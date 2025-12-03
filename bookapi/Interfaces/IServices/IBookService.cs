using System.Text.Json;
using bookapi.Dtos.Books;
using bookapi.Dtos.OpenLibrary;
using bookapi.Models;
using bookapi.Query;

namespace bookapi.Interfaces.IServices
{
    public interface IBookService
    {
        BookDto CreateLocalBook(CreateBookDto createBookDto);
        IEnumerable<BookModel> GetAllBooks();
        Task<OpenLibrarySearchDto> SearchBooksAsync(string query, PaginationOptions pagination);
        Task<JsonElement> SearchBookById(string workKey);
    }
}