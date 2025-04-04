﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            this._movieService = movieService;
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> CreateMovieAsync([FromBody] CreateMovieRequest request, CancellationToken token)
        {
            var movie = request.ToMovie();

            var result = await _movieService.CreateMovieAsync(movie, token);
            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
        }
       
        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug
            , LinkGenerator linkGenerator , CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var movie = Guid.TryParse(idOrSlug, out var id) 
                ? await _movieService.GetMovieByIdAsync(id,userId , token) 
                : await _movieService.GetMovieBySlugAsync(idOrSlug, userId, token);

            if (movie is null)
            {
                return NotFound();
            }

            var response = movie.ToReponse();

            response.Links.Add(new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get), values: new { idOrSlug = movie.Id }),
                Rel = "Salf",
                Type = "GET"
            });

            response.Links.Add(new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update), values: new { id = movie.Id }),
                Rel = "Salf",
                Type = "PUT"
            });

            response.Links.Add(new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Delete), values: new { id = movie.Id }),
                Rel = "Salf",
                Type = "DELETE"
            });


            return Ok(response);
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var options = request.MapToOptions().WithUser(userId);

            var movies = await _movieService.GetAllAsync(options, token);
            var movieCount = await _movieService.GetCountAsync(request.Title, request.Year, token);
            var moviesResponse = movies.ToResponse(request.Page, request.PageSize, movieCount);

            return Ok(moviesResponse);
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id,
            [FromBody] UpdateMovieRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var movie = request.ToMovie(id);

            var updatedMovie = await _movieService.UpdateMovieAsync(movie, userId, token);

            if (updatedMovie is null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
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
