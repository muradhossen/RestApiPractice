using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            this._movieRepository = movieRepository;
        }
        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> CreateMovieAsync([FromBody] CreateMovieRequest request)
        {
            var movie = request.ToMovie();

            var result = await _movieRepository.CreateMovieAsync(movie);
            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
        }
        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug)
        {
            var movie = Guid.TryParse(idOrSlug, out var id) ? await _movieRepository.GetMovieByIdAsync(id) : await _movieRepository.GetMovieBySlugAsync(idOrSlug);

            if (movie is null)
            {
                return NotFound();
            }

            return Ok(movie.ToReponse());
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _movieRepository.GetAllAsync();
            return Ok(movies.ToReponse());
        }
        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id,
            [FromBody] UpdateMovieRequest request)
        {
            var movie = request.ToMovie(id);

            var isUpdate = await _movieRepository.UpdateMovieAsync(movie);

            if (!isUpdate)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var isDeleted = await _movieRepository.DeleteByIdAsync(id);

            if (!isDeleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
