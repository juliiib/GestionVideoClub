using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<Rent> Create([FromBody] CreateRentRequest request)
        {
            try
            {
                var rent = rentService.AddRent(request);
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
        public ActionResult<IReadOnlyList<Rent>> GetAll()
        {
            var rents = rentService.GetAllRents();
            if (!rents.Any())
            {
                return NotFound("No rents found.");
            }
            return Ok(rents);
        }

        [HttpGet("{id}")]
        public ActionResult<Rent> GetById([FromRoute] Guid id)
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
            if (!rentService.ReturnCopy(id, request))
            {
                return NotFound("Rent not found.");
            }

            return NoContent();
        }
    }
}
