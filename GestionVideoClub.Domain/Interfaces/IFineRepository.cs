using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Domain.Interfaces
{
    public interface IFineRepository
    {
        void AddFine(Fine fine);
        IReadOnlyList<Fine> GetAllFines();
        Fine? GetFineById(Guid id);
        void SaveChanges();
    }
}
