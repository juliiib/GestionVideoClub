using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.DTOs
{
    public record CreateMovieRequest(string Title, int? Year, int Clasification, string? Country = null);

    public record MovieResponse(Guid Id, string ImdbId, string Name, string Genre, int Duration, int Clasification, int YearRelease, string? Plot, string? PosterUrl, string? Country)
    {
        public static MovieResponse From(Movie movie) =>
            new(movie.Id, movie.ImdbId, movie.Name, movie.Genre, movie.Duration, movie.Clasification, movie.YearRelease, movie.Plot, movie.PosterUrl, movie.Country);
    }
}
