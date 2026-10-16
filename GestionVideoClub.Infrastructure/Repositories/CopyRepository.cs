using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class CopyRepository : ICopyRepository
    {
        private readonly List<Copy> copies = new List<Copy>();
        public void AddCopy(Copy copy) => copies.Add(copy);
        public IReadOnlyList<Copy> GetAllCopies() => copies.AsReadOnly();
        public Copy? GetCopyById(int id) => copies.FirstOrDefault(c => c.ID == id);
        public void UpdateCopyState(Copy copy) => copies[copies.IndexOf(copy)] = copy;
        public void UpdateCopyAvailability(Copy copy) => copies[copies.IndexOf(copy)] = copy;
    }
}
