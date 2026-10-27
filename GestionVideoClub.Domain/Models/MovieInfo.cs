namespace GestionVideoClub.Domain.Models
{
    public record MovieInfo(string ImdbId, string Title, string Genre, int DurationMinutes, int Year, string? Plot, string? PosterUrl);
}
