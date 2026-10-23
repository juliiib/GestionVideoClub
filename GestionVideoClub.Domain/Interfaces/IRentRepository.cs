using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Domain.Interfaces
{
    public interface IRentRepository
    {
        void AddRent(Rent rent);
        IReadOnlyList<Rent> GetAllRents();
        Rent? GetRentById(Guid id);
        void SaveChanges();
    }
}
