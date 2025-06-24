using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Refit;

namespace Movies.Api.Sdk;

public interface IMoviesApi
{
    [Get(ApiEndpoints.Default.Movies.Get)] Task<MovieResponse> GetMovieAsync(string idOrSlug);

    [Get(ApiEndpoints.Default.Movies.GetAll)] Task<MoviesResponse> GetMoviesAsync(GetAllMoviesRequest request);

}
