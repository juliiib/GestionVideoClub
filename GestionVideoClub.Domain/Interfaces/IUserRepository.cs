using GestionVideoClub.Domain.Entities;

namespace GestionVideoClub.Domain.Interfaces
{
    public interface IUserRepository
    {
        Person? GetByEmail(string email);
        bool EmailExists(string email);
    }
}
