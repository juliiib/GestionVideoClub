using GestionVideoClub.Data;
using GestionVideoClub.DTOs;
using GestionVideoClub.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Xml.Linq;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        [HttpPost]
        public ActionResult<Client> Create([FromBody] CreateClientRequest request)
        {
            try
            {
                var client = new Client(request.Name, request.LastName, request.Dni, request.Phone, request.Address);

                ClientRepository.AddClient(client);

                return CreatedAtAction(nameof(GetById), new { id = client.ID }, client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Client>> GetAll()
        {
            var clients = ClientRepository.GetAll();
            if (!clients.Any())
            {
                return NotFound("No clients found.");
            }
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public ActionResult<Client> GetById([FromRoute] int id)
        {
            var client = ClientRepository.GetByID(id);
            if (client == null)
            {
                return NotFound("Client not found.");
            }
            return Ok(client);
        }

        [HttpPatch("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] UpdateClientRequest request)
        {
            if (!ClientRepository.UpdateClientContact(id, request.Phone, request.Address))
            {
                return NotFound("Client not found.");
            }

            return NoContent();
        }
    }
}
