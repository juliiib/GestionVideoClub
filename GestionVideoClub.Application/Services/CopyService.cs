using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Application.Services
{
    public class CopyService : ICopyService
    {
        private readonly ICopyRepository copyRepository;
        private readonly IMovieRepository movieRepository;
        private readonly IFormatRepository formatRepository;

        public CopyService(ICopyRepository copyRepository, IMovieRepository movieRepository, IFormatRepository formatRepository)
        {
            this.copyRepository = copyRepository;
            this.movieRepository = movieRepository;
            this.formatRepository = formatRepository;
        }

        public Copy AddCopy(CreateCopyRequest request)
        {
            var movie = movieRepository.GetMovieById(request.MovieId);
            if (movie == null)
            {
                throw new ArgumentException("Movie not found.", nameof(request.MovieId));
            }

            var format = formatRepository.GetFormatById(request.FormatId);
            if (format == null)
            {
                throw new ArgumentException("Format not found.", nameof(request.FormatId));
            }

            var copy = new Copy(request.InternalCode, request.State, request.Location, request.IsAvailable, movie, format);
            copyRepository.AddCopy(copy);
            return copy;
        }

        public IReadOnlyList<Copy> GetAllCopies() => copyRepository.GetAllCopies();

        public Copy? GetCopyById(int id) => copyRepository.GetCopyById(id);

        public bool UpdateCopyState(int id, UpdateCopyStateRequest request)
        {
            var copy = copyRepository.GetCopyById(id);
            if (copy == null)
            {
                return false;
            }
            copy.SetState(request.State);
            copyRepository.UpdateCopyState(copy);
            return true;
        }

        public bool UpdateCopyAvailability(int id, UpdateCopyAvailabilityRequest request)
        {
            var copy = copyRepository.GetCopyById(id);
            if (copy == null)
            {
                return false;
            }
            copy.SetAvailability(request.IsAvailable);
            copyRepository.UpdateCopyAvailability(copy);
            return true;
        }
    }
}
