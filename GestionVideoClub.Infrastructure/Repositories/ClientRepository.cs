using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly List<Client> clients = new List<Client>();
        public void AddClient(Client client) => clients.Add(client);
        public IReadOnlyList<Client> GetAllClients() => clients.AsReadOnly();
        public Client? GetClientById(int id) => clients.FirstOrDefault(c => c.ID == id);
        public bool RemoveClient(Client client) => clients.Remove(client);
        public void UpdateClient(Client client) => clients[clients.IndexOf(client)] = client;
    }
}

