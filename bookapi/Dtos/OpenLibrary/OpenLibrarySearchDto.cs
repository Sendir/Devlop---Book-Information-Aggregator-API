using System.Text.Json.Serialization;

namespace bookapi.Dtos.OpenLibrary
{
    public class OpenLibrarySearchDto : PagedResponseDto
    {
        [JsonPropertyName("docs")]
        public IEnumerable<Doc> Docs { get; set; } = Enumerable.Empty<Doc>();
    }

    public class Doc
    {
        [JsonPropertyName("author_key")]
        public IEnumerable<string> KeyAuthor { get; set; } = Enumerable.Empty<string>();

        [JsonPropertyName("author_name")]
        public IEnumerable<string> Author { get; set; } = Enumerable.Empty<string>();

        [JsonPropertyName("key")]
        public string KeyBook { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("language")]
        public IEnumerable<string> Language { get; set; } = Enumerable.Empty<string>();

        [JsonPropertyName("first_publish_year")]
        public int FirstPublishYear { get; set; }

        [JsonPropertyName("edition_count")]
        public int EditionCount { get; set; }
    }
}