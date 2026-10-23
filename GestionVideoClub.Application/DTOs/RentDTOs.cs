namespace GestionVideoClub.Application.DTOs
{
    public record CreateRentRequest(DateTime RentDate, DateTime ExpectedReturnDate, Guid CopyId, Guid ClientId, Guid EmployeeId);
    public record ReturnRentRequest(DateTime ReturnDate);
}
