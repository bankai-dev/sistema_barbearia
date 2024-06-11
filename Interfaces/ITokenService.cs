// ITokenService.cs
using sistema_barbearia.Models;

namespace API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
