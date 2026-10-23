using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Domain.Interfaces
{
    public interface IClientRepository
    {
        void AddClient(Client client);
        IReadOnlyList<Client> GetAllClients();
        Client? GetClientById(Guid id);
        void RemoveClient(Client client);
        void SaveChanges();
    }
}
