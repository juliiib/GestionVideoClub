using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;
using GestionVideoClub.Infrastructure.Persistence;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class FineRepository : IFineRepository
    {
        private readonly VideoClubDbContext context;

        public FineRepository(VideoClubDbContext context)
        {
            this.context = context;
        }

        public void AddFine(Fine fine) => context.Fines.Add(fine);
        public IReadOnlyList<Fine> GetAllFines() => context.Fines.ToList();
        public Fine? GetFineById(Guid id) => context.Fines.FirstOrDefault(f => f.Id == id);
        public void SaveChanges() => context.SaveChanges();
    }
}
