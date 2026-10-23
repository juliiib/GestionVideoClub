using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;
using GestionVideoClub.Infrastructure.Persistence;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class RentRepository : IRentRepository
    {
        private readonly VideoClubDbContext context;

        public RentRepository(VideoClubDbContext context)
        {
            this.context = context;
        }

        public void AddRent(Rent rent) => context.Rents.Add(rent);
        public IReadOnlyList<Rent> GetAllRents() => context.Rents.ToList();
        public Rent? GetRentById(Guid id) => context.Rents.FirstOrDefault(r => r.Id == id);
        public void SaveChanges() => context.SaveChanges();
    }
}
