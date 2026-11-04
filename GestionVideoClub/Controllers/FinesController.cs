using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.EmployeesAndClients)]
    public class FinesController : ControllerBase
    {
        private readonly IFineService fineService;

        public FinesController(IFineService fineService)
        {
            this.fineService = fineService;
        }

        [HttpPost]
        [Authorize(Policy = Policies.EmployeesOnly)]
        public ActionResult<FineResponse> Create([FromBody] CreateFineRequest request)
        {
            try
            {
                var fine = fineService.AddFine(request);
                return CreatedAtAction(nameof(GetById), new { id = fine.Id }, fine);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<FineResponse>> GetAll()
        {
            var fines = fineService.GetAllFines();
            if (!fines.Any())
            {
                return NotFound("No fines found.");
            }
            return Ok(fines);
        }

        [HttpGet("{id}")]
        public ActionResult<FineResponse> GetById([FromRoute] Guid id)
        {
            var fine = fineService.GetFineById(id);
            if (fine == null)
            {
                return NotFound("Fine not found.");
            }
            return Ok(fine);
        }

        [HttpPatch("{id}/pay")]
        [Authorize(Policy = Policies.EmployeesOnly)]
        public ActionResult PayFine([FromRoute] Guid id)
        {
            try
            {
                if (!fineService.PayFine(id))
                {
                    return NotFound("Fine not found.");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
