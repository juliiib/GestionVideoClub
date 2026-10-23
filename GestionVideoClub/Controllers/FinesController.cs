using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinesController : ControllerBase
    {
        private readonly IFineService fineService;

        public FinesController(IFineService fineService)
        {
            this.fineService = fineService;
        }

        [HttpPost]
        public ActionResult<Fine> Create([FromBody] CreateFineRequest request)
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
        public ActionResult<IReadOnlyList<Fine>> GetAll()
        {
            var fines = fineService.GetAllFines();
            if (!fines.Any())
            {
                return NotFound("No fines found.");
            }
            return Ok(fines);
        }

        [HttpGet("{id}")]
        public ActionResult<Fine> GetById([FromRoute] Guid id)
        {
            var fine = fineService.GetFineById(id);
            if (fine == null)
            {
                return NotFound("Fine not found.");
            }
            return Ok(fine);
        }

        [HttpPatch("{id}/pay")]
        public ActionResult PayFine([FromRoute] Guid id)
        {
            if (!fineService.PayFine(id))
            {
                return NotFound("Fine not found.");
            }

            return NoContent();
        }
    }
}
