using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class RentRepository : IRentRepository
    {
        private readonly List<Rent> rents = new List<Rent>();
        public void AddRent(Rent rent) => rents.Add(rent);
        public IReadOnlyList<Rent> GetAllRents() => rents.AsReadOnly();
        public Rent? GetRentById(int id) => rents.FirstOrDefault(r => r.ID == id);
        public void UpdateRent(Rent rent) => rents[rents.IndexOf(rent)] = rent;
    }
}
