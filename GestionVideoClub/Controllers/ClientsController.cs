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
        private readonly IClientService ClientRepository;

        public ClientsController(IClientService clientService)
        {
            ClientRepository = clientService;
        }

        [HttpPost]
        public ActionResult<Client> Create([FromBody] CreateClientRequest request)
        {
            try
            {
                Client client = ClientRepository.AddClient(request);

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
            var clients = ClientRepository.GetAllClients();
            if (!clients.Any())
            {
                return NotFound("No clients found.");
            }
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public ActionResult<Client> GetById([FromRoute] int id)
        {
            var client = ClientRepository.GetClientById(id);
            if (client == null)
            {
                return NotFound("Client not found.");
            }
            return Ok(client);
        }

        [HttpPatch("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] UpdateClientRequest request)
        {
            if (!ClientRepository.UpdateClient(id, request))
            {
                return NotFound("Client not found.");
            }

            return NoContent();
        }
    }
}
