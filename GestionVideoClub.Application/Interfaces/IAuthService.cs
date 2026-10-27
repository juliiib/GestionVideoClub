using GestionVideoClub.Application.DTOs;

namespace GestionVideoClub.Application.Interfaces
{
    public interface IAuthService
    {
        LoginResponse? Login(LoginRequest request);
    }
}
