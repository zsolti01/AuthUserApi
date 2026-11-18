using AuthUserApi.Models;
using AuthUserApi.Models.DTOs;
using AuthUserApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZstdSharp.Unsafe;

namespace AuthUserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AuthuserContext _context;
        private readonly PasswordHasher _password;
        private readonly GenerateToken _generateToken;

        public UsersController(AuthuserContext context, PasswordHasher password, GenerateToken generateToken)
        {
            _context = context;
            _password = password;
            generateToken = generateToken;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("allUser")]
        public async Task<IActionResult> AllUser()
        {
            var users = await _context.Users.ToListAsync();
            if (users is not null)
            {
                return StatusCode(200, new { message = "Sikeres lekérdezés!", result = users });
            }
            return StatusCode(400, new { message = "Sikertelen lekérdezés!", result = users });
        }

        [HttpGet("byId")]
        public async Task<IActionResult> UserById([FromQuery] Guid id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (user is not null)
            {
                return StatusCode(200, new { message = "Sikeres lekérdezés!", result = user });
            }
            return StatusCode(404, new { message = "Sikertelen lekérdezés! Nincs találat.", result = user });
        }

        [HttpPost("newUser")]
        public async Task<IActionResult> NewUser([FromBody] NewUserDTO newUserDTO)
        {
            var (pass, salt) = _password.HashPassword(newUserDTO.Password);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = newUserDTO.Name,
                Email = newUserDTO.Email,
                Password = pass,
                Salt = salt,
                RegTime = DateTime.Now
            };

            if (user is not null)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return StatusCode(201, new { message = "Sikeres felvétel!", result = user });
            }
            return StatusCode(400, new { message = "Sikertelen felvétel!", result = user });
        }

        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateUser([FromQuery] Guid id, UpdateUserDTO updateUserDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is not null)
            {
                user.Name = updateUserDTO.Name;
                user.Email = updateUserDTO.Email;
                user.Password = updateUserDTO.Password;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return StatusCode(200, new { message = "Sikeres frissítés!", result = user });
            }
            return StatusCode(404, new { message = "Sikertelen frissítés!", result = user });
        }

        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user is not null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return StatusCode(200, new { message = "Sikeres törlés!", result = user.Name });
            }
            return StatusCode(404, new { message = "Sikertelen törlés!", result = user });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDTO)
        {
            var login = await _context.Users.Include(u => u.Roles).SingleOrDefaultAsync(u => u.Name == userLoginDTO.Name && u.Password == userLoginDTO.Password);
            if (login is null) return StatusCode(404, new { message = "Nincs találat", result = login });
            var isValid = _password.Verify(userLoginDTO.Password!, login.Password!, login.Salt!);
            if (isValid)
            {
                var newToken = _generateToken.Token(login.Id, login.Email, login.Roles);
                return StatusCode(200, new { message = "Sikeres bejelentkezés", token = newToken });
            }
            return StatusCode(401, new { message = "Hibás név vagy jelszó", result = login });
        }
    }
}
