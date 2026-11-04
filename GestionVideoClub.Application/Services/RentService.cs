using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Application.Services
{
    public class RentService : IRentService
    {
        private readonly IRentRepository rentRepository;
        private readonly ICopyRepository copyRepository;
        private readonly IClientRepository clientRepository;
        private readonly IEmployeeRepository employeeRepository;

        public RentService(IRentRepository rentRepository, ICopyRepository copyRepository, IClientRepository clientRepository, IEmployeeRepository employeeRepository)
        {
            this.rentRepository = rentRepository;
            this.copyRepository = copyRepository;
            this.clientRepository = clientRepository;
            this.employeeRepository = employeeRepository;
        }

        public RentResponse AddRent(CreateRentRequest request, Guid employeeId)
        {
            var copy = copyRepository.GetCopyById(request.CopyId)
                ?? throw new KeyNotFoundException("Copy not found.");

            if (!copy.IsAvailable)
            {
                throw new InvalidOperationException("The copy is not available.");
            }

            var client = clientRepository.GetClientById(request.ClientId)
                ?? throw new KeyNotFoundException("Client not found.");

            var employee = employeeRepository.GetEmployeeById(employeeId)
                ?? throw new KeyNotFoundException("Employee not found.");

            var rent = new Rent(request.RentDate, request.ExpectedReturnDate, copy, client, employee);
            copy.SetAvailability(false);

            rentRepository.AddRent(rent);
            rentRepository.SaveChanges();
            return RentResponse.From(rent);
        }

        public IReadOnlyList<RentResponse> GetAllRents() => rentRepository.GetAllRents().Select(RentResponse.From).ToList();

        public RentResponse? GetRentById(Guid id)
        {
            var rent = rentRepository.GetRentById(id);
            return rent == null ? null : RentResponse.From(rent);
        }

        public bool ReturnCopy(Guid id, ReturnRentRequest request)
        {
            var rent = rentRepository.GetRentById(id);
            if (rent == null)
            {
                return false;
            }

            rent.Return(request.ReturnDate);
            rentRepository.SaveChanges();
            return true;
        }
    }
}
