using GestionVideoClub.Data;
using GestionVideoClub.DTOs;
using GestionVideoClub.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        [HttpPost]
        public ActionResult<Movie> Create([FromBody] CreateMovieRequest request)
        {
            try
            {
                var movie = new Movie(request.Name, request.Genre, request.Duration, request.Clasification, request.YearRelease);
               
                MovieRepository.AddMovie(movie);
                
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
            var movies = MovieRepository.GetAll();
            if (!movies.Any())
            {
                return NotFound("No movies found.");
            }
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public ActionResult<Movie> GetById([FromRoute] int id)
        {
            var movie = MovieRepository.GetByID(id);
            if (movie == null)
            {
                return NotFound("Movie not found.");
            }
            return Ok(movie);
        }
    }
}
