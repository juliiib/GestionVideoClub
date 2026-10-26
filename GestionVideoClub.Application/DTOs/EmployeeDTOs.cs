using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.DTOs
{
    public record CreateEmployeeRequest(string Name, string LastName, int Dni, string Phone, string Address, string Email, string Password, string Shift, decimal Salary);
    public record UpdateEmployeeContactRequest(string? Phone, string? Address);
    public record UpdateEmployeeJobDetailsRequest(string? Shift, decimal? Salary);

    public record EmployeeResponse(Guid Id, string Name, string LastName, int Dni, string Phone, string Address, string Email, string Shift, decimal Salary)
    {
        public static EmployeeResponse From(Employee employee) =>
            new(employee.Id, employee.Name, employee.LastName, employee.DNI, employee.Phone, employee.Address, employee.Email, employee.Shift, employee.Salary);
    }
}
