using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IFormatService
    {
        Format AddFormat(CreateFormatRequest request);
        IReadOnlyList<Format> GetAllFormats();
        Format? GetFormatById(int id);
    }
}
