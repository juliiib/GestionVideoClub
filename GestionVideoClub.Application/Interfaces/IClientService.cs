using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IClientService
    {
        Client AddClient(CreateClientRequest request);
        IReadOnlyList<Client> GetAllClients();
        Client? GetClientById(Guid id);
        bool UpdateClient(Guid id, UpdateClientRequest request);
    }
}
