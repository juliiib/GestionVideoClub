using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.Interfaces
{
    public interface ICopyService
    {
        Copy AddCopy(CreateCopyRequest request);
        IReadOnlyList<Copy> GetAllCopies();
        Copy? GetCopyById(Guid id);
        bool UpdateCopyState(Guid id, UpdateCopyStateRequest request);
        bool UpdateCopyAvailability(Guid id, UpdateCopyAvailabilityRequest request);
    }
}
