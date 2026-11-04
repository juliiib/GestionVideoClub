using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.DTOs
{
    public record CreateRentRequest(DateTime RentDate, DateTime ExpectedReturnDate, Guid CopyId, Guid ClientId);
    public record ReturnRentRequest(DateTime ReturnDate);

    public record RentResponse(
        Guid Id,
        DateTime RentDate,
        DateTime ExpectedReturnDate,
        DateTime? ReturnDate,
        Rent.RentState State,
        Guid CopyId,
        string CopyInternalCode,
        string MovieName,
        Guid ClientId,
        string ClientFullName,
        string ClientEmail,
        Guid EmployeeId,
        string EmployeeFullName,
        Guid? FineId)
    {
        public static RentResponse From(Rent rent) =>
            new(
                rent.Id,
                rent.RentDate,
                rent.ExpectedReturnDate,
                rent.ReturnDate,
                rent.State,
                rent.CopyId,
                rent.Copy.InternalCode,
                rent.Copy.Movie.Name,
                rent.ClientId,
                $"{rent.Client.Name} {rent.Client.LastName}",
                rent.Client.Email,
                rent.EmployeeId,
                $"{rent.Employee.Name} {rent.Employee.LastName}",
                rent.Fine?.Id);
    }
}
