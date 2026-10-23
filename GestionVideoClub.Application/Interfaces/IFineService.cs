using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IFineService
    {
        Fine AddFine(CreateFineRequest request);
        IReadOnlyList<Fine> GetAllFines();
        Fine? GetFineById(Guid id);
        bool PayFine(Guid id);
    }
}
