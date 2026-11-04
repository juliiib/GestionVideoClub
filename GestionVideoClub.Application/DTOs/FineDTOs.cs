using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.DTOs
{
    public record CreateFineRequest(Fine.FineMotive Motive, decimal Amount, DateTime Date, Guid RentId);

    public record FineResponse(Guid Id, Fine.FineMotive Motive, decimal Amount, DateTime Date, Fine.FineState State, Guid RentId, Guid ClientId, string ClientFullName)
    {
        public static FineResponse From(Fine fine) =>
            new(fine.Id, fine.Motive, fine.Amount, fine.Date, fine.State, fine.RentId, fine.Rent.ClientId, $"{fine.Rent.Client.Name} {fine.Rent.Client.LastName}");
    }
}
