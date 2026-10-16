using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.DTOs
{
    public record CreateCopyRequest(string InternalCode, Copy.CopyState State, string Location, bool IsAvailable, Guid MovieId, Guid FormatId);
    public record UpdateCopyStateRequest(Copy.CopyState State);
    public record UpdateCopyAvailabilityRequest(bool IsAvailable);
}
