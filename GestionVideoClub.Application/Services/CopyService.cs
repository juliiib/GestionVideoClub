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

        public CopyResponse AddCopy(CreateCopyRequest request)
        {
            var movie = movieRepository.GetMovieById(request.MovieId)
                ?? throw new KeyNotFoundException("Movie not found.");

            var format = formatRepository.GetFormatById(request.FormatId)
                ?? throw new KeyNotFoundException("Format not found.");

            var copy = new Copy(request.InternalCode, request.State, request.Location, request.IsAvailable, movie, format);
            copyRepository.AddCopy(copy);
            copyRepository.SaveChanges();
            return CopyResponse.From(copy);
        }

        public IReadOnlyList<CopyResponse> GetAllCopies() => copyRepository.GetAllCopies().Select(CopyResponse.From).ToList();

        public CopyResponse? GetCopyById(Guid id)
        {
            var copy = copyRepository.GetCopyById(id);
            return copy == null ? null : CopyResponse.From(copy);
        }

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
