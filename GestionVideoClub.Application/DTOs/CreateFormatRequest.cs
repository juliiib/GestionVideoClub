using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.DTOs
{
    public record CreateFormatRequest(string Name, int ImageQuality, int AdditionalCost);

    public record FormatResponse(Guid Id, string Name, int ImageQuality, int AdditionalCost)
    {
        public static FormatResponse From(Format format) =>
            new(format.Id, format.Name, format.ImageQuality, format.AdditionalCost);
    }
}
