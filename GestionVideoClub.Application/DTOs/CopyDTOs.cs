using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.DTOs
{
    public record CreateCopyRequest(string InternalCode, Copy.CopyState State, string Location, bool IsAvailable, Guid MovieId, Guid FormatId);
    public record UpdateCopyStateRequest(Copy.CopyState State);
    public record UpdateCopyAvailabilityRequest(bool IsAvailable);

    public record CopyResponse(Guid Id, string InternalCode, Copy.CopyState State, string Location, bool IsAvailable, Guid MovieId, string MovieName, Guid FormatId, string FormatName)
    {
        public static CopyResponse From(Copy copy) =>
            new(copy.Id, copy.InternalCode, copy.State, copy.Location, copy.IsAvailable, copy.MovieId, copy.Movie.Name, copy.FormatId, copy.Format.Name);
    }
}
