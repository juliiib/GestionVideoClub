using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.EmployeesOnly)]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpPost]
        public ActionResult<EmployeeResponse> Create([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                var employee = employeeService.AddEmployee(request);

                return CreatedAtAction(nameof(GetById), new { id = employee.Id }, EmployeeResponse.From(employee));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<EmployeeResponse>> GetAll()
        {
            var employees = employeeService.GetAllEmployees();
            if (!employees.Any())
            {
                return NotFound("No employees found.");
            }
            return Ok(employees.Select(EmployeeResponse.From));
        }

        [HttpGet("{id}")]
        public ActionResult<EmployeeResponse> GetById([FromRoute] Guid id)
        {
            var employee = employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound("Employee not found.");
            }
            return Ok(EmployeeResponse.From(employee));
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
