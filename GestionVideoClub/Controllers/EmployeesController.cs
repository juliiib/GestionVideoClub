using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService EmployeeRepository;

        public EmployeesController(IEmployeeService employeeService)
        {
            EmployeeRepository = employeeService;
        }

        [HttpPost]
        public ActionResult<Employee> Create([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                Employee employee = EmployeeRepository.AddEmployee(request);

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
            var employees = EmployeeRepository.GetAllEmployees();
            if (!employees.Any())
            {
                return NotFound("No employees found.");
            }
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public ActionResult<Employee> GetById([FromRoute] int id)
        {
            var employee = EmployeeRepository.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }
            return Ok(employee);
        }

        [HttpPatch("{id}/contact")]
        public ActionResult UpdateContact([FromRoute] int id, [FromBody] UpdateEmployeeContactRequest request)
        {
            if (!EmployeeRepository.UpdateEmployeeContact(id, request))
            {
                return NotFound("Employee not found.");
            }

            return NoContent();
        }

        [HttpPatch("{id}/job-details")]
        public ActionResult UpdateJobDetails([FromRoute] int id, [FromBody] UpdateEmployeeJobDetailsRequest request)
        {
            if (!EmployeeRepository.UpdateEmployeeJobDetails(id, request))
            {
                return NotFound("Employee not found.");
            }

            return NoContent();
        }
    }
}
