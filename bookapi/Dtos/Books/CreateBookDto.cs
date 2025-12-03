using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace bookapi.Dtos.Books
{
    public class CreateBookDto
    {
        [JsonPropertyName("title")]
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("author")]
        [Required(ErrorMessage = "Author is required.")]
        public string Author { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("published_year")]
        [Range(1, int.MaxValue, ErrorMessage = "Published year must be a positive number.")]
        public int PublishedYear { get; set; }
    }
}