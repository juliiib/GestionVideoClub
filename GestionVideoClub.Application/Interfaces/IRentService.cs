using GestionVideoClub.Application.DTOs;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IRentService
    {
        RentResponse AddRent(CreateRentRequest request, Guid employeeId);
        IReadOnlyList<RentResponse> GetAllRents();
        RentResponse? GetRentById(Guid id);
        bool ReturnCopy(Guid id, ReturnRentRequest request);
    }
}
