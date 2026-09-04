using GestionVideoClub.Models;

namespace GestionVideoClub.Data
{
    public class CopyRepository
    {
        private static readonly List<Copy> copies = new List<Copy>();

        public static void AddCopy(Copy copy) => copies.Add(copy);

        public static IReadOnlyList<Copy> GetAll() => copies.AsReadOnly();

        public static Copy? GetByID(int id) => copies.FirstOrDefault(c => c.ID == id);

        public static bool UpdateCopyState(int id, Copy.CopyState newState)
        {
            var copy = GetByID(id);
            if (copy == null) return false;
            
            copy.SetState(newState);
            return true;
        }

        public static bool UpdateCopyAvailability(int id, bool newAvailability)
        {
            var copy = GetByID(id);
            if (copy == null) return false;

            copy.SetAvailability(newAvailability);
            return true;
        }
    }
}