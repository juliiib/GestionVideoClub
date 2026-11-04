using GestionVideoClub.Application.DTOs;

namespace GestionVideoClub.Application.Interfaces
{
    public interface ICopyService
    {
        CopyResponse AddCopy(CreateCopyRequest request);
        IReadOnlyList<CopyResponse> GetAllCopies();
        CopyResponse? GetCopyById(Guid id);
        bool UpdateCopyState(Guid id, UpdateCopyStateRequest request);
        bool UpdateCopyAvailability(Guid id, UpdateCopyAvailabilityRequest request);
    }
}
