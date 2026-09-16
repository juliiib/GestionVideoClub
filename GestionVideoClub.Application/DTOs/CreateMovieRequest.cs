namespace GestionVideoClub.Application.DTOs
{
    public record CreateMovieRequest(string Name, string Genre, int Duration, int Clasification, int YearRelease);
}
