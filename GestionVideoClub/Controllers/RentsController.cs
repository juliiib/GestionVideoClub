using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentsController : ControllerBase
    {
        private readonly IRentService RentRepository;

        public RentsController(IRentService rentService)
        {
            RentRepository = rentService;
        }

        [HttpPost]
        public ActionResult<Rent> Create([FromBody] CreateRentRequest request)
        {
            try
            {
                var rent = RentRepository.AddRent(request);
                return CreatedAtAction(nameof(GetById), new { id = rent.ID }, rent);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Rent>> GetAll()
        {
            var rents = RentRepository.GetAllRents();
            if (!rents.Any())
            {
                return NotFound("No rents found.");
            }
            return Ok(rents);
        }

        [HttpGet("{id}")]
        public ActionResult<Rent> GetById([FromRoute] int id)
        {
            var rent = RentRepository.GetRentById(id);
            if (rent == null)
            {
                return NotFound("Rent not found.");
            }
            return Ok(rent);
        }

        [HttpPatch("{id}/return")]
        public ActionResult ReturnCopy([FromRoute] int id, [FromBody] ReturnRentRequest request)
        {
            if (!RentRepository.ReturnCopy(id, request))
            {
                return NotFound("Rent not found.");
            }

            return NoContent();
        }
    }
}
