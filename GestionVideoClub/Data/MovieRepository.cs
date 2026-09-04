using GestionVideoClub.Models;

namespace GestionVideoClub.Data
{
    public static class MovieRepository
    {
        private static readonly List<Movie> movies = new List<Movie>();

        public static void AddMovie(Movie movie) => movies.Add(movie);

        public static IReadOnlyList<Movie> GetAll() => movies.AsReadOnly();

        public static Movie? GetByID(int id) => movies.FirstOrDefault(m => m.ID == id);
 
    }
}
