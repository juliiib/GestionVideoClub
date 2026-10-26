using GestionVideoClub.Domain.Entities;
using GestionVideoClub.Domain.Interfaces;
using GestionVideoClub.Infrastructure.Persistence;

namespace GestionVideoClub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly VideoClubDbContext context;

        public UserRepository(VideoClubDbContext context)
        {
            this.context = context;
        }

        public Person? GetByEmail(string email) => context.Persons.FirstOrDefault(p => p.Email == email);

        public bool EmailExists(string email) => context.Persons.Any(p => p.Email == email);
    }
}
