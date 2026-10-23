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
            var movie = movieRepository.GetMovieById(request.MovieId)
                ?? throw new KeyNotFoundException("Movie not found.");

            var format = formatRepository.GetFormatById(request.FormatId)
                ?? throw new KeyNotFoundException("Format not found.");

            var copy = new Copy(request.InternalCode, request.State, request.Location, request.IsAvailable, movie, format);
            copyRepository.AddCopy(copy);
            copyRepository.SaveChanges();
            return copy;
        }

        public IReadOnlyList<Copy> GetAllCopies() => copyRepository.GetAllCopies();

        public Copy? GetCopyById(Guid id) => copyRepository.GetCopyById(id);

        public bool UpdateCopyState(Guid id, UpdateCopyStateRequest request)
        {
            var copy = copyRepository.GetCopyById(id);
            if (copy == null)
            {
                return false;
            }
            copy.SetState(request.State);
            copyRepository.SaveChanges();
            return true;
        }

        public bool UpdateCopyAvailability(Guid id, UpdateCopyAvailabilityRequest request)
        {
            var copy = copyRepository.GetCopyById(id);
            if (copy == null)
            {
                return false;
            }
            copy.SetAvailability(request.IsAvailable);
            copyRepository.SaveChanges();
            return true;
        }
    }
}
