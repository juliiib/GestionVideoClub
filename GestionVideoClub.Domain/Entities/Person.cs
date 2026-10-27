namespace GestionVideoClub.Domain.Entities
{
    public abstract class Person
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string LastName { get; private set; }
        public int DNI { get; private set; }
        public string Phone { get; private set; }
        public string Address { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }

        public Person(string name, string lastName, int dni, string phone, string address, string email, string passwordHash)
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
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));
            }

            Id = Guid.NewGuid();
            Name = name;
            LastName = lastName;
            DNI = dni;
            Phone = phone;
            Address = address;
            Email = email;
            PasswordHash = passwordHash;
        }

        protected Person()
        {
            Name = string.Empty;
            LastName = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
        }

        public void UpdatePhone(string newPhone)
        {
            if (string.IsNullOrWhiteSpace(newPhone))
            {
                throw new ArgumentException("New phone cannot be null or empty.", nameof(newPhone));
            }
            Phone = newPhone;
        }

        public void UpdateAddress(string newAddress)
        {
            if (string.IsNullOrWhiteSpace(newAddress))
            {
                throw new ArgumentException("New address cannot be null or empty.", nameof(newAddress));
            }
            Address = newAddress;
        }
    }
}
