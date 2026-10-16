using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Domain.Interfaces
{
    public interface IFormatRepository
    {
        void AddFormat(Format format);
        IReadOnlyList<Format> GetAllFormats();
        Format? GetFormatById(Guid id);
        void SaveChanges();
    }
}
