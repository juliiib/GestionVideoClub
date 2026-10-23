using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;
using GestionVideoClub.Infrastructure.Persistence;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class CopyRepository : ICopyRepository
    {
        private readonly VideoClubDbContext context;

        public CopyRepository(VideoClubDbContext context)
        {
            this.context = context;
        }

        public void AddCopy(Copy copy) => context.Copies.Add(copy);
        public IReadOnlyList<Copy> GetAllCopies() => context.Copies.ToList();
        public Copy? GetCopyById(Guid id) => context.Copies.FirstOrDefault(c => c.Id == id);
        public void SaveChanges() => context.SaveChanges();
    }
}
