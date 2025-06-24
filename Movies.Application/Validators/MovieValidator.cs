using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories; 

namespace Movies.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IMovieRepository _movieRepository;

    public MovieValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;

        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.Now.Year);
        RuleFor(x => x.Genres).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();

        RuleFor(x => x.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This movie already exist in the system.");
    }

    private async Task<bool> ValidateSlug(Movie movie, string arg1, CancellationToken token)
    {
        var existingMovie = await _movieRepository.GetMovieBySlugAsync(movie.Slug);

        if (existingMovie != null)
        {
            return existingMovie.Id == movie.Id;
        }

        return existingMovie is null;
    }
}
