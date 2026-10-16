using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IRentService
    {
        Rent AddRent(CreateRentRequest request);
        IReadOnlyList<Rent> GetAllRents();
        Rent? GetRentById(Guid id);
        bool ReturnCopy(Guid id, ReturnRentRequest request);
    }
}
