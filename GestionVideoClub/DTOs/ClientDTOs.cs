namespace GestionVideoClub.DTOs
{
   public record CreateClientRequest(string Name, string LastName, int Dni, string Phone, string Address);

   public record UpdateClientRequest(string? Phone, string? Address);
}
    