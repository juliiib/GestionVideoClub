using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Application.Services
{
    public class FormatService : IFormatService
    {
        private readonly IFormatRepository formatRepository;

        public FormatService(IFormatRepository formatRepository)
        {
            this.formatRepository = formatRepository;
        }

        public Format AddFormat(CreateFormatRequest request)
        {
            var format = new Format(request.Name, request.ImageQuality, request.AdditionalCost);
            formatRepository.AddFormat(format);
            return format;
        }

        public IReadOnlyList<Format> GetAllFormats() => formatRepository.GetAllFormats();

        public Format? GetFormatById(int id) => formatRepository.GetFormatById(id);
    }
}
