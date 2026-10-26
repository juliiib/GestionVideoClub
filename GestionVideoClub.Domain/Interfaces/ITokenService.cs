namespace GestionVideoClub.Domain.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Guid userId, string email, string role);
    }
}
