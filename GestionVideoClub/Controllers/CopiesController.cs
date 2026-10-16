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
        private readonly ICopyService CopyRepository;

        public CopiesController(ICopyService copyService)
        {
            CopyRepository = copyService;
        }

        [HttpPost]
        public ActionResult<Copy> Create([FromBody] CreateCopyRequest request)
        {
            try
            {
                var copy = CopyRepository.AddCopy(request);
                return CreatedAtAction(nameof(GetById), new { id = copy.ID }, copy);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Copy>> GetAll()
        {
            var copies = CopyRepository.GetAllCopies();
            if (!copies.Any())
            {
                return NotFound("No copies found.");
            }
            return Ok(copies);
        }

        [HttpGet("{id}")]
        public ActionResult<Copy> GetById([FromRoute] int id)
        {
            var copy = CopyRepository.GetCopyById(id);
            if (copy == null)
            {
                return NotFound("Copy not found.");
            }
            return Ok(copy);
        }

        [HttpPatch("{id}/state")]
        public ActionResult UpdateState([FromRoute] int id, [FromBody] UpdateCopyStateRequest request)
        {
            if (!CopyRepository.UpdateCopyState(id, request))
            {
                return NotFound("Copy not found.");
            }

            return NoContent();
        }

        [HttpPatch("{id}/availability")]
        public ActionResult UpdateAvailability([FromRoute] int id, [FromBody] UpdateCopyAvailabilityRequest request)
        {
            if (!CopyRepository.UpdateCopyAvailability(id, request))
            {
                return NotFound("Copy not found.");
            }

            return NoContent();
        }
    }
}
