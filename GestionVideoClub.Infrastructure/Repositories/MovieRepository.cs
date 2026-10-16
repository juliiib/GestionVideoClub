using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly List<Movie> movies = new List<Movie>();
        public void AddMovie(Movie movie) => movies.Add(movie);
        public IReadOnlyList<Movie> GetAllMovies() => movies.AsReadOnly();
        public Movie? GetMovieById(int id) => movies.FirstOrDefault(m => m.ID == id);
    }
}
