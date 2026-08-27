namespace GestionVideoClub.DTOs
{
    public record CreateEmployeeRequest(string Name, string LastName, int Dni, string Phone, string Address, string Shift, decimal Salary);
    public record UpdateEmployeeContactRequest(string? Phone, string? Address);
    public record UpdateEmployeeJobDetailsRequest(string? Shift, decimal? Salary);
    
}
