using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Domain.Interfaces
{
    public interface ICopyRepository
    {
        void AddCopy(Copy copy);
        IReadOnlyList<Copy> GetAllCopies();
        Copy? GetCopyById(Guid id);
        void SaveChanges();
    }
}
