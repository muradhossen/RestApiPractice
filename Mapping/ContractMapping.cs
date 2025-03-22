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
                YearOfRelease = movie.YearOfRelease,
                Rating = movie.Rating,
                UserRating = movie.UserRating
            };
        }

        public static MoviesResponse ToResponse(this IEnumerable<Movie> movies, int Page, int PageSize, int movieCount)
        {
            return new MoviesResponse
            {
                Items = movies.Select(c => c.ToReponse()),
                Page = Page,
                PageSize = PageSize,
                Total = movieCount
            };
        }
        public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings)
        {
            return ratings.Select(x => new MovieRatingResponse
            {
                Rating = x.Rating,
                Slug = x.Slug,
                MovieId = x.MovieId
            });
        }

        public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequest request)
        {
            return new GetAllMoviesOptions
            {
                Title = request.Title,
                YearOfRelease = request.Year,
                SortField = request.SortBy?.Trim('+', '-'),
                SortOrder = request.SortBy is null ? SortOrder.Unsorted :
                request.SortBy.StartsWith('-') ? SortOrder.Descending : SortOrder.Ascending,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options,
            Guid? userId)
        {
            options.UserId = userId;
            return options;
        }
    }
}
