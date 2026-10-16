using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.DTOs
{
    public record CreateFineRequest(Fine.FineMotive Motive, decimal Amount, DateTime Date, Guid RentId);
}
