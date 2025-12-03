using System.Text.Json;
using bookapi.Dtos.Books;
using bookapi.Dtos.OpenLibrary;
using bookapi.Models;

namespace bookapi.Mappers
{
    public static class BookMapper
    {
        public static BookDto ToBookDto(this BookModel book)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                PublishedYear = book.PublishedYear
            };
        }

        public static BookModel ToBookModelFromCreateDto(this CreateBookDto createBookDto)
        {
            return new BookModel
            {
                Title = createBookDto.Title,
                Author = createBookDto.Author,
                Description = createBookDto.Description,
                PublishedYear = createBookDto.PublishedYear
            };
        }

        public static OpenLibraryDetailsDto toOpenLibraryDetailsDtoFromJsonDocument(this JsonElement root)
        {
            return new OpenLibraryDetailsDto
            {
                Key = ExtractAuthors(root),
                Title = root.GetStringSafe("title") ?? string.Empty,
                FirstPublishDate = root.GetStringSafe("first_publish_date") ?? "No publish date aviable",
                Subjects = root.GetStringArray("subjects"),
                Description = ExtractDescription(root)
            };
        }

        private static List<string> ExtractAuthors(JsonElement root)
        {
            var keys = new List<string>();

            if (root.TryGetProperty("authors", out var authorsArray) && authorsArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var authorObj in authorsArray.EnumerateArray())
                {
                    if (!authorObj.TryGetProperty("author", out var author))
                        continue;

                    if (author.TryGetProperty("key", out var keyProp) && keyProp.ValueKind == JsonValueKind.String)
                        keys.Add(keyProp.GetString()!);
                }
            }

            return (keys);
        }


        private static string ExtractDescription(JsonElement root)
        {
            if (!root.TryGetProperty("description", out var desc))
                return "No description available";

            // Case 1: description is a simple string
            if (desc.ValueKind == JsonValueKind.String)
                return desc.GetString() ?? "No description available";

            // Case 2: description is an object → return the "value" field only
            if (desc.ValueKind == JsonValueKind.Object &&
                desc.TryGetProperty("value", out var valueProp) &&
                valueProp.ValueKind == JsonValueKind.String)
            {
                return valueProp.GetString() ?? "No description available";
            }
            return "Failed to parse description";
        }

        public static string? GetStringSafe(this JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String)
                return prop.GetString();

            return null;
        }

        public static int GetIntSafe(this JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out var value))
                return value;

            return 0;
        }

        public static IEnumerable<string> GetStringArray(this JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in prop.EnumerateArray())
                        if (item.ValueKind == JsonValueKind.String)
                            yield return item.GetString()!;
                }
                else if (prop.ValueKind == JsonValueKind.String)
                {
                    yield return prop.GetString()!;
                }
            }
        }
    }
}