using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IEmployeeService
    {
        Employee AddEmployee(CreateEmployeeRequest request);
        IReadOnlyList<Employee> GetAllEmployees();
        Employee? GetEmployeeById(Guid id);
        bool UpdateEmployeeContact(Guid id, UpdateEmployeeContactRequest request);
        bool UpdateEmployeeJobDetails(Guid id, UpdateEmployeeJobDetailsRequest request);
    }
}
