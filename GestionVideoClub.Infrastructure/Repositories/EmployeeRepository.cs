using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;
using GestionVideoClub.Infrastructure.Persistence;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly VideoClubDbContext context;

        public EmployeeRepository(VideoClubDbContext context)
        {
            this.context = context;
        }

        public void AddEmployee(Employee employee) => context.Employees.Add(employee);
        public IReadOnlyList<Employee> GetAllEmployees() => context.Employees.ToList();
        public Employee? GetEmployeeById(Guid id) => context.Employees.FirstOrDefault(e => e.Id == id);
        public void RemoveEmployee(Employee employee) => context.Employees.Remove(employee);
        public void SaveChanges() => context.SaveChanges();
    }
}
