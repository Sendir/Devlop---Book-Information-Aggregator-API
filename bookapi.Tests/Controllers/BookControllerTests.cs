using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using System.Text.Json;
using bookapi.Controllers;
using bookapi.Services;
using bookapi.Dtos.Books;
using bookapi.Dtos.OpenLibrary;
using bookapi.Query;
using bookapi.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace bookapi.Tests.Controllers
{
    public class BookControllerTests
    {
        private readonly Mock<IBookService> _serviceMock;
        private readonly BookController _controller;

        public BookControllerTests()
        {
            _serviceMock = new Mock<IBookService>();
            _controller = new BookController(_serviceMock.Object);
        }

        [Fact]
        public void GetAllLocalBooks_ReturnsOk_WhenBooksExist()
        {
            // Arrange
            var books = new List<Models.BookModel>
            {
                new Models.BookModel { Id = 1, Title = "Book 1", Author = "Author 1" },
                new Models.BookModel { Id = 2, Title = "Book 2", Author = "Author 2" }
            };

            _serviceMock.Setup(s => s.GetAllBooks()).Returns(books);

            // Act
            var result = _controller.GetAllLocalBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnBooks = Assert.IsAssignableFrom<List<BookDto>>(okResult.Value);
            Assert.Equal(2, returnBooks.Count);
        }

        [Fact]
        public void AddLocalBook_ReturnsConflict_WhenBookAlreadyExists()
        {
            // Arrange
            var createDto = new CreateBookDto
            {
                Title = "Existing Book",
                Author = "Author A",
                PublishedYear = 2025
            };

            _serviceMock.Setup(s => s.CreateLocalBook(createDto))
                .Throws(new InvalidOperationException("A book titled 'Existing Book' by Author A already exists."));

            // Act
            var result = _controller.AddLocalBook(createDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            var error = Assert.IsType<BookController.ErrorResponse>(conflictResult.Value);
            Assert.Contains("already exists", error.Message);
        }

        [Fact]
        public async Task GetBookDetails_ReturnsOk_WithValidBook()
        {
            // Arrange
            var id = "OL12345W";

            // Fake JSON returned from the service
            var jsonString = @"{
                ""title"": ""Fake Book"",
                ""authors"": [
                    { ""author"": { ""key"": ""OL12345A"" } }
                ],
                ""first_publish_date"": ""2023"",
                ""subjects"": [""Fiction"", ""Adventure""],
                ""description"": ""A fake book for testing.""
            }";


            using var doc = JsonDocument.Parse(jsonString);
            var jsonElement = doc.RootElement.Clone();

            var serviceMock = new Mock<IBookService>();
            serviceMock.Setup(s => s.SearchBookById(id))
                       .ReturnsAsync(jsonElement);

            var controller = new BookController(serviceMock.Object);

            // Act
            var result = await controller.GetBookDetails(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<OpenLibraryDetailsDto>(okResult.Value);

            Assert.Equal(id, response.KeyWork);
            Assert.Equal("Fake Book", response.Title);
            Assert.Equal("2023", response.FirstPublishDate);
            Assert.Single(response.Key);
            Assert.Equal("OL12345A", response.Key.First());
            Assert.Equal("A fake book for testing.", response.Description);
            Assert.Contains("Fiction", response.Subjects);
            Assert.Contains("Adventure", response.Subjects);
        }
    }
}
