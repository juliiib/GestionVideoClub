using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            this.movieRepository = movieRepository;
        }

        public Movie AddMovie(CreateMovieRequest request)
        {
            var movie = new Movie(request.Name, request.Genre, request.Duration, request.Clasification, request.YearRelease);
            movieRepository.AddMovie(movie);
            movieRepository.SaveChanges();
            return movie;
        }

        public IReadOnlyList<Movie> GetAllMovies() => movieRepository.GetAllMovies();

        public Movie? GetMovieById(Guid id) => movieRepository.GetMovieById(id);
    }
}
