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

        public FormatResponse AddFormat(CreateFormatRequest request)
        {
            var format = new Format(request.Name, request.ImageQuality, request.AdditionalCost);
            formatRepository.AddFormat(format);
            formatRepository.SaveChanges();
            return FormatResponse.From(format);
        }

        public IReadOnlyList<FormatResponse> GetAllFormats() => formatRepository.GetAllFormats().Select(FormatResponse.From).ToList();

        public FormatResponse? GetFormatById(Guid id)
        {
            var format = formatRepository.GetFormatById(id);
            return format == null ? null : FormatResponse.From(format);
        }
    }
}
