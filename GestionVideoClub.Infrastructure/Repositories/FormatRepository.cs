using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class FormatRepository : IFormatRepository
    {
        private readonly List<Format> formats = new List<Format>();
        public void AddFormat(Format format) => formats.Add(format);
        public IReadOnlyList<Format> GetAllFormats() => formats.AsReadOnly();
        public Format? GetFormatById(int id) => formats.FirstOrDefault(f => f.ID == id);
    }
}
