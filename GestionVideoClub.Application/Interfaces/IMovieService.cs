using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IMovieService
    {
        Movie AddMovie(CreateMovieRequest request);
        IReadOnlyList<Movie> GetAllMovies();
        Movie? GetMovieById(int id);
    }
}
