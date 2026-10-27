using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.EmployeesAndClients)]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService movieService;

        public MoviesController(IMovieService movieService)
        {
            this.movieService = movieService;
        }

        [HttpPost]
        [Authorize(Policy = Policies.EmployeesOnly)]
        public async Task<ActionResult<MovieResponse>> Create([FromBody] CreateMovieRequest request)
        {
            try
            {
                var movie = await movieService.AddMovieAsync(request);

                return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "No se pudo consultar OMDb.");
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Movie>> GetAll()
        {
            var movies = movieService.GetAllMovies();
            if (!movies.Any())
            {
                return NotFound("No movies found.");
            }
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public ActionResult<Movie> GetById([FromRoute] Guid id)
        {
            var movie = movieService.GetMovieById(id);
            if (movie == null)
            {
                return NotFound("Movie not found.");
            }
            return Ok(movie);
        }
    }
}
