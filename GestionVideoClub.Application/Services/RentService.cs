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

        public Rent AddRent(CreateRentRequest request)
        {
            var copy = copyRepository.GetCopyById(request.CopyId);
            if (copy == null)
            {
                throw new ArgumentException("Copy not found.", nameof(request.CopyId));
            }

            var client = clientRepository.GetClientById(request.ClientId);
            if (client == null)
            {
                throw new ArgumentException("Client not found.", nameof(request.ClientId));
            }

            var employee = employeeRepository.GetEmployeeById(request.EmployeeId);
            if (employee == null)
            {
                throw new ArgumentException("Employee not found.", nameof(request.EmployeeId));
            }

            var rent = new Rent(request.RentDate, request.ExpectedReturnDate, copy, client, employee);
            rentRepository.AddRent(rent);
            return rent;
        }

        public IReadOnlyList<Rent> GetAllRents() => rentRepository.GetAllRents();

        public Rent? GetRentById(int id) => rentRepository.GetRentById(id);

        public bool ReturnCopy(int id, ReturnRentRequest request)
        {
            var rent = rentRepository.GetRentById(id);
            if (rent == null || rent.State != Rent.RentState.Active)
            {
                return false;
            }

            if (request.ReturnDate > rent.ExpectedReturnDate)
            {
                rent.ReturnCopy(Rent.RentState.Overdue, request.ReturnDate);
            }
            else
            {
                rent.ReturnCopy(Rent.RentState.Returned, request.ReturnDate);
            }

            rentRepository.UpdateRent(rent);
            return true;
        }
    }
}
