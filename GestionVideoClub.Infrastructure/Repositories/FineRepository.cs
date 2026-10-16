using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class FineRepository : IFineRepository
    {
        private readonly List<Fine> fines = new List<Fine>();
        public void AddFine(Fine fine) => fines.Add(fine);
        public IReadOnlyList<Fine> GetAllFines() => fines.AsReadOnly();
        public Fine? GetFineById(int id) => fines.FirstOrDefault(f => f.ID == id);
        public void UpdateFine(Fine fine) => fines[fines.IndexOf(fine)] = fine;
    }
}
