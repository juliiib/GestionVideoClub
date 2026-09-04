using GestionVideoClub.Models;

namespace GestionVideoClub.Data
{
    public class FormatRepository
    {
        private static readonly List<Format> formats = new List<Format>();

        public static void AddFormat(Format format) => formats.Add(format);

        public static IReadOnlyList<Format> GetAll() => formats.AsReadOnly();

        public static Format? GetByID(int id) => formats.FirstOrDefault(f => f.ID == id);
    }
}
