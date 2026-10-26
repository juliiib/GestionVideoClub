using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FormatsController : ControllerBase
    {
        private readonly IFormatService formatService;

        public FormatsController(IFormatService formatService)
        {
            this.formatService = formatService;
        }

        [HttpPost]
        public ActionResult<Format> Create([FromBody] CreateFormatRequest request)
        {
            try
            {
                var format = formatService.AddFormat(request);
                return CreatedAtAction(nameof(GetById), new { id = format.Id }, format);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Format>> GetAll()
        {
            var formats = formatService.GetAllFormats();
            if (!formats.Any())
            {
                return NotFound("No formats found.");
            }
            return Ok(formats);
        }

        [HttpGet("{id}")]
        public ActionResult<Format> GetById([FromRoute] Guid id)
        {
            var format = formatService.GetFormatById(id);
            if (format == null)
            {
                return NotFound("Format not found.");
            }
            return Ok(format);
        }
    }
}
