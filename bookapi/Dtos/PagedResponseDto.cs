using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace bookapi.Dtos
{
    public class PagedResponseDto
    {
        [JsonPropertyName("start")]
        public int Start { get; set; }
        public int PageSize { get; set; }

        [JsonPropertyName("numFound")]
        public int NumFound { get; set; }
        public int TotalPages { get; set; }
    }
}