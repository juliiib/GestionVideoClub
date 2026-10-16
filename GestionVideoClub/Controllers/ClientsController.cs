using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService clientService;

        public ClientsController(IClientService clientService)
        {
            this.clientService = clientService;
        }

        [HttpPost]
        public ActionResult<Client> Create([FromBody] CreateClientRequest request)
        {
            try
            {
                Client client = clientService.AddClient(request);

                return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Client>> GetAll()
        {
            var clients = clientService.GetAllClients();
            if (!clients.Any())
            {
                return NotFound("No clients found.");
            }
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public ActionResult<Client> GetById([FromRoute] Guid id)
        {
            var client = clientService.GetClientById(id);
            if (client == null)
            {
                return NotFound("Client not found.");
            }
            return Ok(client);
        }

        [HttpPatch("{id}")]
        public ActionResult Update([FromRoute] Guid id, [FromBody] UpdateClientRequest request)
        {
            if (!clientService.UpdateClient(id, request))
            {
                return NotFound("Client not found.");
            }

            return NoContent();
        }
    }
}
