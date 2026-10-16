using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormatsController : ControllerBase
    {
        private readonly IFormatService FormatRepository;

        public FormatsController(IFormatService formatService)
        {
            FormatRepository = formatService;
        }

        [HttpPost]
        public ActionResult<Format> Create([FromBody] CreateFormatRequest request)
        {
            try
            {
                var format = FormatRepository.AddFormat(request);
                return CreatedAtAction(nameof(GetById), new { id = format.ID }, format);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Format>> GetAll()
        {
            var formats = FormatRepository.GetAllFormats();
            if (!formats.Any())
            {
                return NotFound("No formats found.");
            }
            return Ok(formats);
        }

        [HttpGet("{id}")]
        public ActionResult<Format> GetById([FromRoute] int id)
        {
            var format = FormatRepository.GetFormatById(id);
            if (format == null)
            {
                return NotFound("Format not found.");
            }
            return Ok(format);
        }
    }
}
