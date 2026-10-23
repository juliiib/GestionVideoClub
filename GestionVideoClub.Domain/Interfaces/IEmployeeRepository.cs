using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        void AddEmployee(Employee employee);
        IReadOnlyList<Employee> GetAllEmployees();
        Employee? GetEmployeeById(Guid id);
        void RemoveEmployee(Employee employee);
        void SaveChanges();
    }
}
