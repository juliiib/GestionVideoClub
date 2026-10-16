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
            employeeRepository.SaveChanges();
            return employee;
        }

        public IReadOnlyList<Employee> GetAllEmployees() => employeeRepository.GetAllEmployees();

        public Employee? GetEmployeeById(Guid id) => employeeRepository.GetEmployeeById(id);

        public bool RemoveEmployee(Guid id)
        {
            var employee = employeeRepository.GetEmployeeById(id);
            if (employee == null)
            {
                return false;
            }
            employeeRepository.RemoveEmployee(employee);
            employeeRepository.SaveChanges();
            return true;
        }

        public bool UpdateEmployeeContact(Guid id, UpdateEmployeeContactRequest request)
        {
            var employee = employeeRepository.GetEmployeeById(id);
            if (employee == null)
            {
                return false;
            }

            if (request.Address != null)
            {
                employee.UpdateAddress(request.Address);
            }
            if (request.Phone != null)
            {
                employee.UpdatePhone(request.Phone);
            }

            employeeRepository.SaveChanges();
            return true;
        }

        public bool UpdateEmployeeJobDetails(Guid id, UpdateEmployeeJobDetailsRequest request)
        {
            var employee = employeeRepository.GetEmployeeById(id);
            if (employee == null)
            {
                return false;
            }

            if (request.Shift != null)
            {
                employee.UpdateShift(request.Shift);
            }
            if (request.Salary.HasValue)
            {
                employee.UpdateSalary(request.Salary.Value);
            }

            employeeRepository.SaveChanges();
            return true;
        }
    }
}
