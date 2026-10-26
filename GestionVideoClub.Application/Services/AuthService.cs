using GestionVideoClub.Application.DTOs;
using GestionVideoClub.Application.Interfaces;
using GestionVideoClub.Domain.Interfaces;

namespace GestionVideoClub.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHashService passwordHashService;
        private readonly ITokenService tokenService;

        public AuthService(IUserRepository userRepository, IPasswordHashService passwordHashService, ITokenService tokenService)
        {
            this.userRepository = userRepository;
            this.passwordHashService = passwordHashService;
            this.tokenService = tokenService;
        }

        public LoginResponse? Login(LoginRequest request)
        {
            var user = userRepository.GetByEmail(request.Email);
            if (user == null || !passwordHashService.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            var token = tokenService.GenerateToken(user.Id, user.Email, user.GetType().Name);
            return new LoginResponse(token);
        }
    }
}
