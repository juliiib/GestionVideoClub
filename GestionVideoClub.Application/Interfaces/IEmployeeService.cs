using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IEmployeeService
    {
        Employee AddEmployee(CreateEmployeeRequest request);
        IReadOnlyList<Employee> GetAllEmployees();
        Employee? GetEmployeeById(int id);
        bool UpdateEmployeeContact(int id, UpdateEmployeeContactRequest request);
        bool UpdateEmployeeJobDetails(int id, UpdateEmployeeJobDetailsRequest request);
    }
}
