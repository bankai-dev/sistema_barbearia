using System.Security.Cryptography;
using System.Text;
using sistema_barbearia.Data;
using sistema_barbearia.DTOs;
using sistema_barbearia.Models;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace sistema_barbearia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AccountController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            _tokenService = tokenService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            if (registerDto == null || string.IsNullOrEmpty(registerDto.Username) || string.IsNullOrEmpty(registerDto.Password))
            {
                return BadRequest("Dados inválidos");
            }

            if (await _userService.UserExists(registerDto.Username))
            {
                return BadRequest("Nome de usuário já é utilizado! ");
            }

            var user = new User
            {
                UserName = registerDto.Username
            };

            var registeredUser = await _userService.RegisterUser(user, registerDto.Password);

            if (registeredUser == null)
                return BadRequest("Registration failed");

            return new UserDto
            {
                Username = registeredUser.UserName,
                Token = _tokenService.CreateToken(registeredUser)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Dados inválidos");
            }
            var authenticatedUser = await _userService.AuthenticateUser(loginDto.Username, loginDto.Password);

            if (authenticatedUser == null)
                return Unauthorized("Invalid Username or Password");

            int userId = authenticatedUser.Id;


            var userDto = new UserDto
            {
                Id = userId,
                Username = authenticatedUser.UserName,
                Token = _tokenService.CreateToken(authenticatedUser)
            };

            return Ok(new
            {
                User = userDto,
                RedirectUrl = userId == 2 ? "/Listagem" : "/ListagemClientes"
            });
        }
    }
}
