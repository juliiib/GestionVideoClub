using GestionVideoClub.Models;

namespace GestionVideoClub.Data
{
    public class RentRepository
    {
        private static readonly List<Rent> rents = new List<Rent>();

        public static void AddRent(Rent rent) => rents.Add(rent);

        public static IReadOnlyList<Rent> GetAll() => rents.AsReadOnly();

        public static Rent? GetByID(int id) => rents.FirstOrDefault(r => r.ID == id);

        public bool ReturnCopy(int rentID, DateTime returnDate)
        {
            var rent = GetByID(rentID);
            if (rent == null || rent.State != Rent.RentState.Active) return false;

            if (returnDate > rent.ExpectedReturnDate)
            {
                rent.ReturnCopy(Rent.RentState.Overdue, returnDate);
            }
            else
            {
                rent.ReturnCopy(Rent.RentState.Returned, returnDate);
            }
            return true;
        }
    }
}
