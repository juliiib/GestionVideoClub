using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Domain.Interfaces
{
    public interface IMovieRepository
    {
        void AddMovie(Movie movie);
        IReadOnlyList<Movie> GetAllMovies();
        Movie? GetMovieById(Guid id);
        bool ExistsByImdbId(string imdbId);
        void SaveChanges();
    }
}
