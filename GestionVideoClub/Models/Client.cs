namespace GestionVideoClub.Models
{
    public class Client : Person
    {
        public Client(string name, string lastName, int dni, string phone, string address)
            : base(name, lastName, dni, phone, address)
        {
        }
    }
}
