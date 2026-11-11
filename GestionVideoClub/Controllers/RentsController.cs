using System.Security.Claims;
using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.EmployeesAndClients)]
    public class RentsController : ControllerBase
    {
        private readonly IRentService rentService;

        public RentsController(IRentService rentService)
        {
            this.rentService = rentService;
        }

        [HttpPost]
        [Authorize(Policy = Policies.EmployeesOnly)]
        public ActionResult<RentResponse> Create([FromBody] CreateRentRequest request)
        {
            // El empleado que registra el alquiler es el logueado, no uno que mande el cliente
            // en el body: se toma del claim de identidad del token ("sub"), que el handler de
            // JWT mapea a ClaimTypes.NameIdentifier.
            var employeeIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (employeeIdClaim == null || !Guid.TryParse(employeeIdClaim, out var employeeId))
            {
                return Unauthorized("No se pudo identificar al empleado autenticado.");
            }

            try
            {
                var rent = rentService.AddRent(request, employeeId);
                return CreatedAtAction(nameof(GetById), new { id = rent.Id }, rent);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<RentResponse>> GetAll()
        {
            var rents = rentService.GetAllRents();
            if (!rents.Any())
            {
                return NotFound("No rents found.");
            }
            return Ok(rents);
        }

        [HttpGet("{id}")]
        public ActionResult<RentResponse> GetById([FromRoute] Guid id)
        {
            var rent = rentService.GetRentById(id);
            if (rent == null)
            {
                return NotFound("Rent not found.");
            }
            return Ok(rent);
        }

        [HttpPatch("{id}/return")]
        [Authorize(Policy = Policies.EmployeesOnly)]
        public ActionResult ReturnCopy([FromRoute] Guid id, [FromBody] ReturnRentRequest request)
        {
            try
            {
                if (!rentService.ReturnCopy(id, request))
                {
                    return NotFound("Rent not found.");
                }

                return NoContent();
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
    }
}
