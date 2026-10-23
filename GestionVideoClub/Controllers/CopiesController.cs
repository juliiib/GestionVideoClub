using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CopiesController : ControllerBase
    {
        private readonly ICopyService copyService;

        public CopiesController(ICopyService copyService)
        {
            this.copyService = copyService;
        }

        [HttpPost]
        public ActionResult<Copy> Create([FromBody] CreateCopyRequest request)
        {
            try
            {
                var copy = copyService.AddCopy(request);
                return CreatedAtAction(nameof(GetById), new { id = copy.Id }, copy);
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
        public ActionResult<IReadOnlyList<Copy>> GetAll()
        {
            var copies = copyService.GetAllCopies();
            if (!copies.Any())
            {
                return NotFound("No copies found.");
            }
            return Ok(copies);
        }

        [HttpGet("{id}")]
        public ActionResult<Copy> GetById([FromRoute] Guid id)
        {
            var copy = copyService.GetCopyById(id);
            if (copy == null)
            {
                return NotFound("Copy not found.");
            }
            return Ok(copy);
        }

        [HttpPatch("{id}/state")]
        public ActionResult UpdateState([FromRoute] Guid id, [FromBody] UpdateCopyStateRequest request)
        {
            if (!copyService.UpdateCopyState(id, request))
            {
                return NotFound("Copy not found.");
            }

            return NoContent();
        }

        [HttpPatch("{id}/availability")]
        public ActionResult UpdateAvailability([FromRoute] Guid id, [FromBody] UpdateCopyAvailabilityRequest request)
        {
            if (!copyService.UpdateCopyAvailability(id, request))
            {
                return NotFound("Copy not found.");
            }

            return NoContent();
        }
    }
}
