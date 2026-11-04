using GestionVideoClub.Application.DTOs;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IMovieService
    {
        Task<MovieResponse> AddMovieAsync(CreateMovieRequest request);
        IReadOnlyList<MovieResponse> GetAllMovies();
        MovieResponse? GetMovieById(Guid id);
    }
}
