using sistema_barbearia.Models;
using System.Threading.Tasks;

public interface IUserService
{
    Task<User> AuthenticateUser(string username, string password);
    Task<User> RegisterUser(User user, string password);

    Task<bool> UserExists(string username);
}
