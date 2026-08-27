using GestionVideoClub.Models;

namespace GestionVideoClub.Data
{
    public class EmployeeRepository
    {
        private static readonly List<Employee> employees = new List<Employee>();

        public static void AddEmployee(Employee employee) => employees.Add(employee);

        public static IReadOnlyList<Employee> GetAll() => employees.AsReadOnly();

        public static Employee? GetByID(int id) => employees.FirstOrDefault(e => e.ID == id);

        public static bool UpdateEmployeePhone(int id, string newPhone)
        {
            var employee = GetByID(id);
            if (employee == null) return false;

            employee.UpdatePhone(newPhone);
            return true;
        }

        public static bool UpdateEmployeeAddress(int id, string newAddress)
        {
            var employee = GetByID(id);
            if (employee == null) return false;

            employee.UpdateAddress(newAddress);
            return true;
        }

        public static bool UpdateEmployeeShift(int id, string newShift)
        {
            var employee = GetByID(id);
            if (employee == null) return false;

            employee.UpdateShift(newShift);
            return true;
        }

        public static bool UpdateEmployeeSalary(int id, decimal newSalary)
        {
            var employee = GetByID(id);
            if (employee == null) return false;

            employee.UpdateSalary(newSalary);
            return true;
        }
    }
}
