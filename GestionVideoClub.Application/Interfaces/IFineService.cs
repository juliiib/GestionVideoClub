using GestionVideoClub.Application.DTOs;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IFineService
    {
        FineResponse AddFine(CreateFineRequest request);
        IReadOnlyList<FineResponse> GetAllFines();
        FineResponse? GetFineById(Guid id);
        bool PayFine(Guid id);
    }
}
