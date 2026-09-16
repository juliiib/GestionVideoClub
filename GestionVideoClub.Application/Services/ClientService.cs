using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public Client AddClient(CreateClientRequest request)
        {
            var client = new Client(request.Name, request.LastName, request.Dni, request.Phone, request.Address);
            clientRepository.AddClient(client);
            return client;
        }

        public IReadOnlyList<Client> GetAllClients()=> clientRepository.GetAllClients();

        public Client? GetClientById(int id) => clientRepository.GetClientById(id);

        public bool RemoveClient(int id)
        {
            var client = clientRepository.GetClientById(id);
            if (client == null)
            {
                return false;
            }
            clientRepository.RemoveClient(client);
            return true;
        }

        public bool UpdateClient(int id, UpdateClientRequest request)
        {
            var client = clientRepository.GetClientById(id);
            if (client == null)
            {
                return false;
            }
            
            client.UpdateAddress(request.Address);
            client.UpdatePhone(request.Phone);

            clientRepository.UpdateClient(client);

            return true;
        }
    }
}
