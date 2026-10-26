namespace GestionVideoClub.Domain.Entities
{
    public class Client : Person
    {
        public Client(string name, string lastName, int dni, string phone, string address, string email, string passwordHash)
            : base(name, lastName, dni, phone, address, email, passwordHash)
        {
        }

        private Client()
        {
        }
    }
}
