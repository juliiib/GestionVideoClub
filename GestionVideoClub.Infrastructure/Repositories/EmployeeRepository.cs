using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private static readonly List<Employee> employees = new List<Employee>();
        public void AddEmployee(Employee employee) => employees.Add(employee);
        public IReadOnlyList<Employee> GetAllEmployees() => employees.AsReadOnly();
        public Employee? GetEmployeeById(int id) => employees.FirstOrDefault(e => e.ID == id);
        public bool RemoveEmployee(Employee employee) => employees.Remove(employee);
        public void UpdateEmployeeContact(Employee employee) => employees[employees.IndexOf(employee)] = employee;
        public void UpdateEmployeeJobDetails(Employee employee) => employees[employees.IndexOf(employee)] = employee;
    }
}