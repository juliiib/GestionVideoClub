using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;
using GestionVideoClub.Infrastructure.Persistence;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly VideoClubDbContext context;

        public MovieRepository(VideoClubDbContext context)
        {
            this.context = context;
        }

        public void AddMovie(Movie movie) => context.Movies.Add(movie);
        public IReadOnlyList<Movie> GetAllMovies() => context.Movies.ToList();
        public Movie? GetMovieById(Guid id) => context.Movies.FirstOrDefault(m => m.Id == id);
        public bool ExistsByImdbId(string imdbId) => context.Movies.Any(m => m.ImdbId == imdbId);
        public void SaveChanges() => context.SaveChanges();
    }
}
