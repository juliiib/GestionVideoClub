using GestionVideoClub.Models;

namespace GestionVideoClub.Data
{
    public class ClientRepository
    {
        private static readonly List<Client> clients = new List<Client>();

        public static void AddClient(Client client) => clients.Add(client);

        public static IReadOnlyList<Client> GetAll() => clients.AsReadOnly();

        public static Client? GetByID(int id) => clients.FirstOrDefault(c => c.ID == id);

        public static bool UpdateClientContact(int id, string newPhone, string newAddress)
        {
            var client = GetByID(id);
            if (client == null) return false;

            if (newPhone != null) client.UpdatePhone(newPhone);
            if (newAddress != null) client.UpdateAddress(newAddress);

            return true;
        }
    }
}
