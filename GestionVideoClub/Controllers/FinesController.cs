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
        private readonly IFineService FineRepository;

        public FinesController(IFineService fineService)
        {
            FineRepository = fineService;
        }

        [HttpPost]
        public ActionResult<Fine> Create([FromBody] CreateFineRequest request)
        {
            try
            {
                var fine = FineRepository.AddFine(request);
                return CreatedAtAction(nameof(GetById), new { id = fine.ID }, fine);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Fine>> GetAll()
        {
            var fines = FineRepository.GetAllFines();
            if (!fines.Any())
            {
                return NotFound("No fines found.");
            }
            return Ok(fines);
        }

        [HttpGet("{id}")]
        public ActionResult<Fine> GetById([FromRoute] int id)
        {
            var fine = FineRepository.GetFineById(id);
            if (fine == null)
            {
                return NotFound("Fine not found.");
            }
            return Ok(fine);
        }

        [HttpPatch("{id}/pay")]
        public ActionResult PayFine([FromRoute] int id)
        {
            if (!FineRepository.PayFine(id))
            {
                return NotFound("Fine not found.");
            }

            return NoContent();
        }
    }
}
