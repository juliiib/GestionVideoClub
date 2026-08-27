namespace GestionVideoClub.Models
{
    public class Fine
    {
        private static int nextID = 0;

        public enum FineMotive {LateReturn, DamagedItem, LostItem, Other}
        public enum FineState { Paid, Unpaid }

        public int ID { get; }
        public FineMotive Motive { get; }
        public decimal Amount { get; }
        public DateTime Date { get; }
        public FineState State { get; private set; }

        public Rent Rent { get; }

        public Fine(FineMotive motive, decimal amount, DateTime date, Rent rent)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
            }
            ID = nextID++;
            Motive = motive;
            Amount = amount;
            Date = date;
            State = FineState.Unpaid;
            Rent = rent ?? throw new ArgumentNullException(nameof(rent), "A fine must belong to a rent.");
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
