using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            this._movieService = movieService;
        }
        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> CreateMovieAsync([FromBody] CreateMovieRequest request, CancellationToken token)
        {
            var movie = request.ToMovie();

            var result = await _movieService.CreateMovieAsync(movie, token);
            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
        }
        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
        {
            var movie = Guid.TryParse(idOrSlug, out var id) ? await _movieService.GetMovieByIdAsync(id) : await _movieService.GetMovieBySlugAsync(idOrSlug);

            if (movie is null)
            {
                return NotFound();
            }

            return Ok(movie.ToReponse());
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll(CancellationToken token)
        {
            var movies = await _movieService.GetAllAsync();
            return Ok(movies.ToReponse());
        }
        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id,
            [FromBody] UpdateMovieRequest request, CancellationToken token)
        {
            var movie = request.ToMovie(id);

            var updatedMovie = await _movieService.UpdateMovieAsync(movie, token);

            if (updatedMovie is null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
        {
            var isDeleted = await _movieService.DeleteByIdAsync(id, token);

            if (!isDeleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
