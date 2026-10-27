using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository movieRepository;
        private readonly IMovieInfoService movieInfoService;

        public MovieService(IMovieRepository movieRepository, IMovieInfoService movieInfoService)
        {
            this.movieRepository = movieRepository;
            this.movieInfoService = movieInfoService;
        }

        public async Task<MovieResponse> AddMovieAsync(CreateMovieRequest request)
        {
            var info = await movieInfoService.SearchByTitleAsync(request.Title, request.Year)
                ?? throw new KeyNotFoundException($"No se encontró la película '{request.Title}' en OMDb.");

            if (movieRepository.ExistsByImdbId(info.ImdbId))
            {
                throw new InvalidOperationException("La película ya está cargada.");
            }

            var movie = new Movie(info.ImdbId, info.Title, info.Genre, info.DurationMinutes, request.Clasification, info.Year, info.Plot, info.PosterUrl);
            movieRepository.AddMovie(movie);
            movieRepository.SaveChanges();
            return MovieResponse.From(movie);
        }

        public IReadOnlyList<Movie> GetAllMovies() => movieRepository.GetAllMovies();

        public Movie? GetMovieById(Guid id) => movieRepository.GetMovieById(id);
    }
}
