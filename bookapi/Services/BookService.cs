using System.Text.Json;
using bookapi.Dtos;
using bookapi.Dtos.Books;
using bookapi.Dtos.OpenLibrary;
using bookapi.Interfaces.IServices;
using bookapi.Mappers;
using bookapi.Models;
using bookapi.Query;

namespace bookapi.Services
{
    public class BookService : IBookService
    {
        private readonly string _fileName = "books.json";
        private readonly List<BookModel> _localBooks;
        private readonly IHttpClientFactory _httpClientFactory;



        public BookService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            try
            {
                var file = File.ReadAllText(_fileName);

                _localBooks = JsonSerializer.Deserialize<List<BookModel>>(file) ?? new List<BookModel>();

            }
            catch (FileNotFoundException fnfe)
            {
                throw new FileNotFoundException($"Failed to find file named {_fileName}", fnfe);
            }
            catch (JsonException je)
            {
                throw new JsonException($"Failed to read file {_fileName} though to bad parsing. Ensure the file contains valid json.", je);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occured during BookService initialization", ex);
            }
        }

        //Local Book Library Methods

        public IEnumerable<BookModel> GetAllBooks()
        {
            return _localBooks;
        }

        public BookDto CreateLocalBook(CreateBookDto createBookDto)
        {
            if (_localBooks.Any(book => book.Title == createBookDto.Title && book.Author == createBookDto.Author))
                throw new InvalidOperationException($"A book titled '{createBookDto.Title}' by {createBookDto.Author} already exists.");

            var book = createBookDto.ToBookModelFromCreateDto();
            book.Id = _localBooks.Count() + 1;
            _localBooks.Add(book);

            SaveBooksToFile();

            return book.ToBookDto();
        }

        private void SaveBooksToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_localBooks, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_fileName, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save books to file {_fileName}", ex);
            }
        }


        // Open Library Methods

        public async Task<OpenLibrarySearchDto> SearchBooksAsync(string query, PaginationOptions pagination)
        {
            var client = _httpClientFactory.CreateClient("OpenLibrary");

            var url = $"search.json?q={Uri.EscapeDataString(query)}&page={pagination.PageNumber}&limit={pagination.PageSize}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OpenLibrarySearchDto>();

            if (result == null)
                throw new InvalidOperationException("OpenLibrary returned null");

            result.PageSize = pagination.PageSize;
            result.TotalPages = (int)Math.Ceiling((double)result.NumFound / pagination.PageSize);

            return result;
        }

        public async Task<JsonElement> SearchBookById(string workKey)
        {
            var client = _httpClientFactory.CreateClient("OpenLibrary");
            var url = $"works/{workKey}.json";

            var responseObj = await client.GetAsync(url);
            responseObj.EnsureSuccessStatusCode();

            // Read raw JSON string
            var responseString = await responseObj.Content.ReadAsStringAsync();

            var root = JsonDocument.Parse(responseString).RootElement;

            return root;
        }
    }
}