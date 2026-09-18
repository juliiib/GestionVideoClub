using GestionVideoClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionVideoClub.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        void AddEmployee(Employee employee);
        IReadOnlyList<Employee> GetAllEmployees();
        Employee? GetEmployeeById(int id);
        bool RemoveEmployee(Employee employee);
        void UpdateEmployeeContact(Employee employee);
        void UpdateEmployeeJobDetails(Employee employee);
    }
}
