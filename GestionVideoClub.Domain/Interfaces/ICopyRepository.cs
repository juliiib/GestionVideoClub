using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Domain.Interfaces
{
    public interface ICopyRepository
    {
        void AddCopy(Copy copy);
        IReadOnlyList<Copy> GetAllCopies();
        Copy? GetCopyById(int id);
        void UpdateCopyState(Copy copy);
        void UpdateCopyAvailability(Copy copy);
    }
}
