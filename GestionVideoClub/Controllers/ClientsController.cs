using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionVideoClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService clientService;

        public ClientsController(IClientService clientService)
        {
            this.clientService = clientService;
        }

        [HttpPost]
        public ActionResult<ClientResponse> Create([FromBody] CreateClientRequest request)
        {
            try
            {
                var client = clientService.AddClient(request);

                return CreatedAtAction(nameof(GetById), new { id = client.Id }, ClientResponse.From(client));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<ClientResponse>> GetAll()
        {
            var clients = clientService.GetAllClients();
            if (!clients.Any())
            {
                return NotFound("No clients found.");
            }
            return Ok(clients.Select(ClientResponse.From));
        }

        [HttpGet("{id}")]
        public ActionResult<ClientResponse> GetById([FromRoute] Guid id)
        {
            var client = clientService.GetClientById(id);
            if (client == null)
            {
                return NotFound("Client not found.");
            }
            return Ok(ClientResponse.From(client));
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
