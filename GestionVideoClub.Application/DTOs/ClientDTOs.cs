using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Application.DTOs
{
   public record CreateClientRequest(string Name, string LastName, int Dni, string Phone, string Address, string Email, string Password);

   public record UpdateClientRequest(string? Phone, string? Address);

   public record ClientResponse(Guid Id, string Name, string LastName, int Dni, string Phone, string Address, string Email)
   {
       public static ClientResponse From(Client client) =>
           new(client.Id, client.Name, client.LastName, client.DNI, client.Phone, client.Address, client.Email);
   }
}
