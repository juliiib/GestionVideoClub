using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService MovieRepository;

        public MoviesController(IMovieService movieService)
        {
            MovieRepository = movieService;
        }

        [HttpPost]
        public ActionResult<Movie> Create([FromBody] CreateMovieRequest request)
        {
            try
            {
                var movie = MovieRepository.AddMovie(request);

                return CreatedAtAction(nameof(GetById), new { id = movie.ID }, movie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Movie>> GetAll()
        {
            var movies = MovieRepository.GetAllMovies();
            if (!movies.Any())
            {
                return NotFound("No movies found.");
            }
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public ActionResult<Movie> GetById([FromRoute] int id)
        {
            var movie = MovieRepository.GetMovieById(id);
            if (movie == null)
            {
                return NotFound("Movie not found.");
            }
            return Ok(movie);
        }
    }
}
