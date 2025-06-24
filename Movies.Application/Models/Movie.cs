using System.Text.RegularExpressions;

namespace Movies.Application.Models
{
    public partial class Movie
    {
        public required Guid Id { get; init; }
        public required string Title { get; set; }
        public required int YearOfRelease { get; set; }
        public required List<string> Genres { get; set; } = new();
        public float? Rating { get; set; }
        public int? UserRating { get; set; }
        public string Slug => GenerateSlug();

        private string GenerateSlug()
        {
            var sluggedTitle = SlugSanitizerRegex().Replace(Title, string.Empty)
                .ToLower().Replace(" ", "-");

            return $"{sluggedTitle}-{YearOfRelease}";
        }

        [GeneratedRegex("[^0-9A-Za-z_-]")]
        private static partial Regex SlugSanitizerRegex();
    }
}
