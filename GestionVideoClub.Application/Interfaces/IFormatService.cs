using GestionVideoClub.Application.DTOs;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IFormatService
    {
        FormatResponse AddFormat(CreateFormatRequest request);
        IReadOnlyList<FormatResponse> GetAllFormats();
        FormatResponse? GetFormatById(Guid id);
    }
}
