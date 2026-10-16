using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IFineService
    {
        Fine AddFine(CreateFineRequest request);
        IReadOnlyList<Fine> GetAllFines();
        Fine? GetFineById(int id);
        bool PayFine(int id);
    }
}
