using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpPost]
        public ActionResult<Employee> Create([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                Employee employee = employeeService.AddEmployee(request);

                return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Employee>> GetAll()
        {
            var employees = employeeService.GetAllEmployees();
            if (!employees.Any())
            {
                return NotFound("No employees found.");
            }
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public ActionResult<Employee> GetById([FromRoute] Guid id)
        {
            var employee = employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }
            return Ok(employee);
        }

        [HttpPatch("{id}/contact")]
        public ActionResult UpdateContact([FromRoute] Guid id, [FromBody] UpdateEmployeeContactRequest request)
        {
            if (!employeeService.UpdateEmployeeContact(id, request))
            {
                return NotFound("Employee not found.");
            }

            return NoContent();
        }

        [HttpPatch("{id}/job-details")]
        public ActionResult UpdateJobDetails([FromRoute] Guid id, [FromBody] UpdateEmployeeJobDetailsRequest request)
        {
            if (!employeeService.UpdateEmployeeJobDetails(id, request))
            {
                return NotFound("Employee not found.");
            }

            return NoContent();
        }
    }
}
