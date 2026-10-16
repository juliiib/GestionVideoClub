using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.DTOs
{
    public record CreateCopyRequest(string InternalCode, Copy.CopyState State, string Location, bool IsAvailable, int MovieId, int FormatId);
    public record UpdateCopyStateRequest(Copy.CopyState State);
    public record UpdateCopyAvailabilityRequest(bool IsAvailable);
}
