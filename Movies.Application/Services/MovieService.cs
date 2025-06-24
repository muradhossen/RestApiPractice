using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IValidator<Movie> _validator;
        private readonly IRatingRepository _ratingRepository;
        private readonly IValidator<GetAllMoviesOptions> _optionsValidators;

        public MovieService(IMovieRepository movieRepository
            , IValidator<Movie> validator
            , IRatingRepository ratingRepository
            , IValidator<GetAllMoviesOptions> optionsValidators)
        {
            this._movieRepository = movieRepository;
            this._validator = validator;
            this._ratingRepository = ratingRepository;
            _optionsValidators = optionsValidators;
        }
        public async Task<bool> CreateMovieAsync(Movie movie, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(movie, token);

            return await _movieRepository.CreateMovieAsync(movie, token);
        }

        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _movieRepository.DeleteByIdAsync(id, token);
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
        {
            await _optionsValidators.ValidateAndThrowAsync(options,token);
            return await _movieRepository.GetAllAsync(options, token);
        }

        public Task<Movie> GetMovieByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
        {
            return _movieRepository.GetMovieByIdAsync(id, userId, token);
        }

        public Task<Movie> GetMovieBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
        {
            return _movieRepository.GetMovieBySlugAsync(slug, userId, token);
        }

        public async Task<Movie?> UpdateMovieAsync(Movie movie, Guid? userId = default, CancellationToken token = default)
        {
            await _validator.ValidateAndThrowAsync(movie, token);

            var isExist = await _movieRepository.ExistsByIdAsync(movie.Id, token);

            if (!isExist)
            {
                return null;
            }

            await _movieRepository.UpdateMovieAsync(movie, token);

            if (!userId.HasValue)
            {
                var rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
                movie.Rating = rating;
                return movie;
            }

            var ratings = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
            movie.Rating = ratings.Rating;
            movie.UserRating = ratings.UserRating;

            return movie;
        }

        public Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
        {
            return _ratingRepository.GetRatingsForUserAsync(userId, token);
        }

        public async Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default)
        {
            return await _movieRepository.GetCountAsync(title, yearOfRelease, token);
        }
    }
}
