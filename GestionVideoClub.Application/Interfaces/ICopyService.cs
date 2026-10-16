using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.Interfaces
{
    public interface ICopyService
    {
        Copy AddCopy(CreateCopyRequest request);
        IReadOnlyList<Copy> GetAllCopies();
        Copy? GetCopyById(int id);
        bool UpdateCopyState(int id, UpdateCopyStateRequest request);
        bool UpdateCopyAvailability(int id, UpdateCopyAvailabilityRequest request);
    }
}
