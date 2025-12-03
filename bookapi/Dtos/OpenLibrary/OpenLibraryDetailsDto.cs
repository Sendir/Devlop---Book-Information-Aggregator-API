using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace bookapi.Dtos.OpenLibrary
{
    public class OpenLibraryDetailsDto
    {
        [JsonPropertyName("work_key")]
        public string KeyWork { get; set; } = string.Empty;

        [JsonPropertyName("author_key")]
        public IEnumerable<string> Key { get; set; } = Enumerable.Empty<string>();

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("first_publish_date")]
        public string FirstPublishDate { get; set; } = string.Empty;

        [JsonPropertyName("subjects")]
        public IEnumerable<string> Subjects { get; set; } = Enumerable.Empty<string>();

        [JsonPropertyName("description")]
        public string? Description { get; set; } = "No description avaiable";
    }
}