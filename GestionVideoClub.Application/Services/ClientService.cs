using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository clientRepository;
        private readonly IUserRepository userRepository;
        private readonly IPasswordHashService passwordHashService;

        public ClientService(IClientRepository clientRepository, IUserRepository userRepository, IPasswordHashService passwordHashService)
        {
            this.clientRepository = clientRepository;
            this.userRepository = userRepository;
            this.passwordHashService = passwordHashService;
        }

        public Client AddClient(CreateClientRequest request)
        {
            if (userRepository.EmailExists(request.Email))
            {
                throw new InvalidOperationException("Email already exists.");
            }

            var passwordHash = passwordHashService.Hash(request.Password);
            var client = new Client(request.Name, request.LastName, request.Dni, request.Phone, request.Address, request.Email, passwordHash);
            clientRepository.AddClient(client);
            clientRepository.SaveChanges();
            return client;
        }

        public IReadOnlyList<Client> GetAllClients()=> clientRepository.GetAllClients();

        public Client? GetClientById(Guid id) => clientRepository.GetClientById(id);

        public bool RemoveClient(Guid id)
        {
            var client = clientRepository.GetClientById(id);
            if (client == null)
            {
                return false;
            }
            clientRepository.RemoveClient(client);
            clientRepository.SaveChanges();
            return true;
        }

        public bool UpdateClient(Guid id, UpdateClientRequest request)
        {
            var client = clientRepository.GetClientById(id);
            if (client == null)
            {
                return false;
            }

            if (request.Address != null)
            {
                client.UpdateAddress(request.Address);
            }
            if (request.Phone != null)
            {
                client.UpdatePhone(request.Phone);
            }

            clientRepository.SaveChanges();

            return true;
        }
    }
}
