using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;
using GestionVideoClub.Infrastructure.Persistence;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class FormatRepository : IFormatRepository
    {
        private readonly VideoClubDbContext context;

        public FormatRepository(VideoClubDbContext context)
        {
            this.context = context;
        }

        public void AddFormat(Format format) => context.Formats.Add(format);
        public IReadOnlyList<Format> GetAllFormats() => context.Formats.ToList();
        public Format? GetFormatById(Guid id) => context.Formats.FirstOrDefault(f => f.Id == id);
        public void SaveChanges() => context.SaveChanges();
    }
}
