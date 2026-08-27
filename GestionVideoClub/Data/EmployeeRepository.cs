using GestionVideoClub.Models;

namespace GestionVideoClub.Data
{
    public class EmployeeRepository
    {
        private static readonly List<Employee> employees = new List<Employee>();

        public static void AddEmployee(Employee employee) => employees.Add(employee);

        public static IReadOnlyList<Employee> GetAll() => employees.AsReadOnly();

        public static Employee? GetByID(int id) => employees.FirstOrDefault(e => e.ID == id);

        public static bool UpdateEmployeeContact(int id, string? newPhone, string? newAddress)
        {
            var employee = GetByID(id);
            if (employee == null) return false;

            if (newPhone != null) employee.UpdatePhone(newPhone);
            if (newAddress != null) employee.UpdateAddress(newAddress);

            return true;
        }

        public static bool UpdateEmployeeJobDetails(int id, string? newShift, decimal? newSalary)
        {
            var employee = GetByID(id);
            if (employee == null) return false;

            if (newShift != null) employee.UpdateShift(newShift);
            if (newSalary.HasValue && newSalary > 0) employee.UpdateSalary(newSalary.Value);

            return true;
        }
    }
}