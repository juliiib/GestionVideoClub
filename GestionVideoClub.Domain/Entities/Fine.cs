namespace GestionVideoClub.Domain.Entities
{
    public class Fine
    {
        public enum FineMotive {LateReturn, DamagedItem, LostItem, Other}
        public enum FineState { Paid, Unpaid }

        public Guid Id { get; private set; }
        public FineMotive Motive { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public FineState State { get; private set; }

        public Guid RentId { get; private set; }
        public Rent Rent { get; private set; }

        public Fine(FineMotive motive, decimal amount, DateTime date, Rent rent)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
            }
            Id = Guid.NewGuid();
            Motive = motive;
            Amount = amount;
            Date = date;
            State = FineState.Unpaid;
            Rent = rent ?? throw new ArgumentNullException(nameof(rent), "A fine must belong to a rent.");
            RentId = rent.Id;
        }

        private Fine()
        {
            Rent = null!;
        }

        public void PayFine()
        {
            if (State == FineState.Paid)
            {
                throw new InvalidOperationException("The fine has already been paid.");
            }
            State = FineState.Paid;
        }
    }
}
