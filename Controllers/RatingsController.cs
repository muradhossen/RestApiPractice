using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Controllers;
using Movies.Api.Mapping;
using Movies.Application.Services;

namespace Movies.Contracts.Requests
{
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [Authorize]
        [HttpPut(ApiEndpoints.Movies.Rate)]
        public async Task<IActionResult> RateMovieAsync([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var result = await _ratingService.RateMovieAsync(id, request.Rating, userId!.Value, token);

            return result ? Ok() : NotFound();
        }
    }
}
