namespace GestionVideoClub.Application.DTOs
{
    public record CreateRentRequest(DateTime RentDate, DateTime ExpectedReturnDate, int CopyId, int ClientId, int EmployeeId);
    public record ReturnRentRequest(DateTime ReturnDate);
}
