using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IClientService
    {
        Client AddClient(CreateClientRequest request);
        IReadOnlyList<Client> GetAllClients();
        Client? GetClientById(int id);
        bool UpdateClient(int id, UpdateClientRequest request);
    }
}
