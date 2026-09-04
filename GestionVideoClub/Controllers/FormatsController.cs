using GestionVideoClub.Data;
using GestionVideoClub.DTOs;
using GestionVideoClub.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormatsController : ControllerBase
    {
        [HttpPost]
        public ActionResult<Format> Create([FromBody] CreateFormatRequest request)
        {
            try
            {
                var format = new Format(request.Name, request.ImageQuality, request.AdditionalCost);
                FormatRepository.AddFormat(format);
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
            var formats = FormatRepository.GetAll();
            if (!formats.Any())
            {
                return NotFound("No formats found.");
            }
            return Ok(formats);
        }

        [HttpGet("{id}")]
        public ActionResult<Format> GetById([FromRoute] int id)
        {
            var format = FormatRepository.GetByID(id);
            if (format == null)
            {
                return NotFound("Format not found.");
            }
            return Ok(format);
        }
    }
}
