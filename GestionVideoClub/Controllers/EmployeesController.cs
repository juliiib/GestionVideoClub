using GestionVideoClub.Data;
using GestionVideoClub.DTOs;
using GestionVideoClub.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        [HttpPost]
        public ActionResult<Employee> Create([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                var employee = new Employee(request.Name, request.LastName, request.Dni, request.Phone, request.Address, request.Shift, request.Salary);

                EmployeeRepository.AddEmployee(employee);

                return CreatedAtAction(nameof(GetById), new { id = employee.ID }, employee);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Employee>> GetAll()
        {
            var employees = EmployeeRepository.GetAll();
            if (!employees.Any())
            {
                return NotFound("No employees found.");
            }
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public ActionResult<Employee> GetById([FromRoute] int id)
        {
            var employee = EmployeeRepository.GetByID(id);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }
            return Ok(employee);
        }

        [HttpPatch("{id}/contact")]
        public ActionResult UpdateContact([FromRoute] int id, [FromBody] UpdateEmployeeContactRequest request)
        {
            if (!EmployeeRepository.UpdateEmployeeContact(id, request.Phone, request.Address))
            {
                return NotFound("Employee not found.");
            }

            return NoContent();
        }

        [HttpPatch("{id}/job-details")]
        public ActionResult UpdateJobDetails([FromRoute] int id, [FromBody] UpdateEmployeeJobDetailsRequest request)
        {
            if (!EmployeeRepository.UpdateEmployeeJobDetails(id, request.Shift, request.Salary))
            {
                return NotFound("Employee not found.");
            }

            return NoContent();
        }
    }
}
