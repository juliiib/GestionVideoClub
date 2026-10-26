using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IMovieService
    {
        Task<MovieResponse> AddMovieAsync(CreateMovieRequest request);
        IReadOnlyList<Movie> GetAllMovies();
        Movie? GetMovieById(Guid id);
    }
}
