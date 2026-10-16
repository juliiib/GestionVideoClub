namespace GestionVideoClub.Domain.Entities
{
    public class Rent
    {
        public enum RentState { Active, Returned, Overdue }

        public Guid Id { get; private set; }
        public DateTime RentDate { get; private set; }
        public DateTime ExpectedReturnDate { get; private set; }
        public DateTime? ReturnDate { get; private set; }
        public RentState State { get; private set; }

        public Guid CopyId { get; private set; }
        public Guid ClientId { get; private set; }
        public Guid EmployeeId { get; private set; }

        public Copy Copy { get; private set; }
        public Client Client { get; private set; }
        public Employee Employee { get; private set; }
        public Fine? Fine { get; private set; }

        public Rent(DateTime rentDate, DateTime expectedReturnDate, Copy copy, Client client, Employee employee)
        {
            if (rentDate > expectedReturnDate)
            {
                throw new ArgumentException("Rent date cannot be later than expected return date.");
            }

            Id = Guid.NewGuid();
            RentDate = rentDate;
            ExpectedReturnDate = expectedReturnDate;
            State = RentState.Active;
            Copy = copy ?? throw new ArgumentNullException(nameof(copy), "Copy cannot be null.");
            CopyId = copy.Id;
            Client = client ?? throw new ArgumentNullException(nameof(client), "Client cannot be null.");
            ClientId = client.Id;
            Employee = employee ?? throw new ArgumentNullException(nameof(employee), "Employee cannot be null.");
            EmployeeId = employee.Id;
        }

        private Rent()
        {
            Copy = null!;
            Client = null!;
            Employee = null!;
        }

        public void ReturnCopy(RentState newState, DateTime returnDate)
        {
            State = newState;
            ReturnDate = returnDate;
        }
    }
}
