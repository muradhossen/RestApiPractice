using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers.V2
{
    [Authorize]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        } 
 

        [HttpGet(ApiEndpoints.V2.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug
            , LinkGenerator linkGenerator, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var movie = Guid.TryParse(idOrSlug, out var id)
                ? await _movieService.GetMovieByIdAsync(id, userId, token)
                : await _movieService.GetMovieBySlugAsync(idOrSlug, userId, token);

            if (movie is null)
            {
                return NotFound();
            }

            var response = movie.ToReponse();
            return Ok(response);
        }

 
    }
}
