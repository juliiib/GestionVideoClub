namespace GestionVideoClub.Models
{
    public class Rent
    {
        private static int nextID = 0;

        public enum RentState { Active, Returned, Overdue }

        public int ID { get; }
        public DateTime RentDate { get; }
        public DateTime ExpectedReturnDate { get; }
        public DateTime ReturnDate { get; private set; }
        public RentState State { get; private set; }

        public Copy Copy { get; }
        public Client Client { get; }
        public Employee Employee { get; }
        public Fine Fine { get; private set; }

        public Rent(DateTime rentDate, DateTime expectedReturnDate, Copy copy, Client client, Employee employee)
        {
            if (rentDate > expectedReturnDate)
            {
                throw new ArgumentException("Rent date cannot be later than expected return date.");
            }

            ID = nextID++;
            RentDate = rentDate;
            ExpectedReturnDate = expectedReturnDate;
            State = RentState.Active;
            Copy = copy ?? throw new ArgumentNullException(nameof(copy), "Copy cannot be null.");
            Client = client ?? throw new ArgumentNullException(nameof(client), "Client cannot be null.");
            Employee = employee ?? throw new ArgumentNullException(nameof(employee), "Employee cannot be null.");
        }


    }
}
