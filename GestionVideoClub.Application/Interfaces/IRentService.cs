using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IRentService
    {
        Rent AddRent(CreateRentRequest request);
        IReadOnlyList<Rent> GetAllRents();
        Rent? GetRentById(int id);
        bool ReturnCopy(int id, ReturnRentRequest request);
    }
}
