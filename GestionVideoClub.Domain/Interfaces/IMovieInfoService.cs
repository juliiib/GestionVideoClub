using GestionVideoClub.Domain.Models;

namespace GestionVideoClub.Domain.Interfaces
{
    public interface IMovieInfoService
    {
        Task<MovieInfo?> SearchByTitleAsync(string title, int? year);
    }
}
