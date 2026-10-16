using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;
using GestionVideoClub.Infrastructure.Persistence;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly VideoClubDbContext context;

        public ClientRepository(VideoClubDbContext context)
        {
            this.context = context;
        }

        public void AddClient(Client client) => context.Clients.Add(client);
        public IReadOnlyList<Client> GetAllClients() => context.Clients.ToList();
        public Client? GetClientById(Guid id) => context.Clients.FirstOrDefault(c => c.Id == id);
        public void RemoveClient(Client client) => context.Clients.Remove(client);
        public void SaveChanges() => context.SaveChanges();
    }
}
