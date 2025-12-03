using System;
using System.Linq;
using Moq;
using Xunit;
using System.Net.Http.Json;
using Moq.Protected;
using bookapi.Services;
using bookapi.Dtos;
using bookapi.Dtos.Books;
using bookapi.Dtos.OpenLibrary;
using bookapi.Query;


namespace bookapi.Tests.Services
{
    public class BookServiceTests
    {


        [Fact]
        public void GetAllBooks_CheckIfReturnsMoreThanOne()
        {
            // Arrange
            var service = CreateNewService();

            // Act
            var books = service.GetAllBooks();

            // Assert
            Assert.NotNull(books);
            Assert.True(books.Any(), "Expected at least one book");
        }
        // Helper method to create a fresh BookService per test
        private static BookService CreateNewService()
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
                       .Returns(new HttpClient());

            return new BookService(mockFactory.Object);
        }

        [Fact]
        public async Task SearchBooksAsync_ReturnsCorrectResult()
        {
            // Arrange
            var pagination = new PaginationOptions { PageNumber = 1, PageSize = 10 };
            var query = "test query";

            var fakeResponse = new OpenLibrarySearchDto
            {
                NumFound = 25,
                Docs = new[]
                {
            new Doc { Title = "Book 1" },
            new Doc { Title = "Book 2" }
        }
            };

            var httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = JsonContent.Create(fakeResponse)
            };

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            var client = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://openlibrary.org/")
            };

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient("OpenLibrary"))
                       .Returns(client);

            var service = new BookService(factoryMock.Object);

            // Act
            var result = await service.SearchBooksAsync(query, pagination);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pagination.PageSize, result.PageSize);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal(2, result.Docs.Count());
            Assert.Equal("Book 1", result.Docs.First().Title);

            // Verify request URL
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.AbsolutePath.Contains("search.json") &&
                    req.RequestUri.Query.Contains("page=1") &&
                    req.RequestUri.Query.Contains("limit=10")
                ),
                ItExpr.IsAny<CancellationToken>()
                        );
        }
    }
}
