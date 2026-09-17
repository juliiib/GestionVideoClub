using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        public Employee AddEmployee(CreateEmployeeRequest request)
        {
            var employee = new Employee(request.Name, request.LastName, request.Dni, request.Phone, request.Address, request.Shift, request.Salary);
            employeeRepository.AddEmployee(employee);
            return employee;
        }

        public IReadOnlyList<Employee> GetAllEmployees() => employeeRepository.GetAllEmployees();

        public Employee? GetEmployeeById(int id) => employeeRepository.GetEmployeeById(id);

        public bool RemoveEmployee(int id)
        {
            var employee = employeeRepository.GetEmployeeById(id);
            if (employee == null)
            {
                return false;
            }
            employeeRepository.RemoveEmployee(employee);
            return true;
        }

        public bool UpdateEmployeeContact(int id, UpdateEmployeeContactRequest request)
        {
            var employee = employeeRepository.GetEmployeeById(id);
            if (employee == null)
            {
                return false;
            }
            employee.UpdateAddress(request.Address);
            employee.UpdatePhone(request.Phone);
            employeeRepository.UpdateEmployeeContact(employee);
            return true;
        }

        public bool UpdateEmployeeJobDetails(int id, UpdateEmployeeJobDetailsRequest request)
        {
            var employee = employeeRepository.GetEmployeeById(id);
            if (employee == null)
            {
                return false;
            }
            employee.UpdateShift(request.Shift);
            employee.UpdateSalary(request.Salary.Value);
            employeeRepository.UpdateEmployeeJobDetails(employee);
            return true;
        }
    }
}
