using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using System.Runtime.CompilerServices;

namespace Movies.Api.Mapping
{
    public static class ContractMapping
    {
        public static Movie ToMovie(this CreateMovieRequest request)
        {
            return new Movie
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Genres = request.Genres.ToList(),
                YearOfRelease = request.YearOfRelease
            };
        }
        public static Movie ToMovie(this UpdateMovieRequest request, Guid id)
        {
            return new Movie
            {
                Id = id,
                Title = request.Title,
                Genres = request.Genres.ToList(),
                YearOfRelease = request.YearOfRelease
            };
        }
        public static MovieResponse ToReponse(this Movie movie)
        {
            return new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                Slug = movie.Slug,
                Genres = movie.Genres.ToList(),
                YearOfRelease = movie.YearOfRelease
            };
        }

        public static MoviesResponse ToReponse(this IEnumerable<Movie> movies)
        {
            return new MoviesResponse
            {
                Items = movies.Select(c => c.ToReponse())
            };
        }
    }
}
