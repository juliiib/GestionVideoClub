namespace GestionVideoClub.Models
{
    public abstract class Person
    {
        private static int nextID = 0;

        public int ID { get; }
        public string Name { get; }
        public string LastName { get; }
        public int DNI { get; }
        public string Phone { get; private set; }
        public string Address { get; private set; }

        public Person(string name, string lastName, int dni, string phone, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));
            }
            if (dni <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dni), "DNI must be greater than zero.");
            }
            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Phone cannot be null or empty.", nameof(phone));
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address cannot be null or empty.", nameof(address));
            }

            ID = nextID++;
            Name = name;
            LastName = lastName;
            DNI = dni;
            Phone = phone;
            Address = address;
        }
    }
}
