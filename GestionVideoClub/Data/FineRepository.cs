using GestionVideoClub.Models;

namespace GestionVideoClub.Data
{
    public class FineRepository
    {
        private static readonly List<Fine> fines = new List<Fine>();

        public static void AddFine(Fine fine) => fines.Add(fine);

        public static IReadOnlyList<Fine> GetAll() => fines.AsReadOnly();

        public static Fine? GetByID(int id) => fines.FirstOrDefault(f => f.ID == id);

        public static bool PayFine(int id)
        {
            var fine = GetByID(id);
            if (fine == null) return false;
            
            try
            {
                fine.PayFine();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
